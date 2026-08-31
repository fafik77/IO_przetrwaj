using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using Przetrwaj.Application.Settings;

namespace Przetrwaj.Application.Services;

public class EmailCustomSmtpService : IEmailSenderMultiple
{
	private readonly EmailSettingsCustomSmtp _customSmtpSettings;
	private readonly ILogger<EmailCustomSmtpService> _logger;

	public EmailCustomSmtpService(IOptions<EmailSettings> options, ILogger<EmailCustomSmtpService> logger)
	{
		_customSmtpSettings = options.Value.CustomSmtp;
		_logger = logger;
	}

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		if (!_customSmtpSettings.IsConfigured)
			throw new InvalidOperationException("Custom SMTP email service is not properly configured.");

		var message = CreateBaseMessage(subject, htmlMessage);
		message.To.Add(MailboxAddress.Parse(email));

		await SendMimeMessageAsync(message);
	}

	public async Task SendBlindEmailToMultipleAsync(IEnumerable<string> emails, string subject, string htmlMessage)
	{
		if (!_customSmtpSettings.IsConfigured)
			throw new InvalidOperationException("Custom SMTP email service is not properly configured.");

		var message = CreateBaseMessage(subject, htmlMessage);
		foreach (var email in emails)
		{
			if (!string.IsNullOrWhiteSpace(email))
				message.Bcc.Add(MailboxAddress.Parse(email));
		}

		await SendMimeMessageAsync(message);
	}

	private MimeMessage CreateBaseMessage(string subject, string htmlMessage)
	{
		var message = new MimeMessage();

		var fromAddress = !string.IsNullOrWhiteSpace(_customSmtpSettings.SenderDisplayName)
			? new MailboxAddress(_customSmtpSettings.SenderDisplayName, _customSmtpSettings.UserEmail!)
			: MailboxAddress.Parse(_customSmtpSettings.UserEmail!);

		message.From.Add(fromAddress);
		message.Subject = subject;

		// Message-ID with custom domain or domain from sender email
		var domain = !string.IsNullOrWhiteSpace(_customSmtpSettings.Domain)
			? _customSmtpSettings.Domain
			: _customSmtpSettings.UserEmail?.Split('@').LastOrDefault();

		message.MessageId = !string.IsNullOrWhiteSpace(domain)
			? MimeUtils.GenerateMessageId(domain)
			: MimeUtils.GenerateMessageId();

		var bodyBuilder = new BodyBuilder
		{
			HtmlBody = htmlMessage,
			TextBody = htmlMessage
		};

		message.Body = bodyBuilder.ToMessageBody();
		return message;
	}

	private async Task SendMimeMessageAsync(MimeMessage message)
	{
		using var client = new SmtpClient();

		var secureSocketOptions = _customSmtpSettings.SSL
			? SecureSocketOptions.SslOnConnect
			: (_customSmtpSettings.SmtpPort == 587 ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

		try
		{
			await client.ConnectAsync(_customSmtpSettings.SmtpServer, _customSmtpSettings.SmtpPort, secureSocketOptions);
			await client.AuthenticateAsync(_customSmtpSettings.UserEmail, _customSmtpSettings.UserPassword);
			await client.SendAsync(message);
			await client.DisconnectAsync(true);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send email via Custom SMTP ({SmtpServer}:{SmtpPort})", _customSmtpSettings.SmtpServer, _customSmtpSettings.SmtpPort);
			throw;
		}
	}
}
