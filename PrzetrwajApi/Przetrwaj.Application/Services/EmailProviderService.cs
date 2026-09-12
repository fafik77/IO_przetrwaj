using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Settings;

namespace Przetrwaj.Application.Services;

public class EmailProviderService : IEmailSenderMultiple
{
	private readonly EmailCustomSmtpService _customSmtpService;
	private readonly EmailAzureService _azureService;
	private readonly EmailSettings _emailSettings;
	private readonly ILogger<EmailProviderService> _logger;

	public EmailProviderService(
		EmailCustomSmtpService customSmtpService,
		EmailAzureService azureService,
		IOptions<EmailSettings> options,
		ILogger<EmailProviderService> logger)
	{
		_customSmtpService = customSmtpService;
		_azureService = azureService;
		_emailSettings = options.Value;
		_logger = logger;
	}

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		if (_emailSettings.CustomSmtp.IsConfigured)
		{
			_logger.LogInformation("Routing email to Custom SMTP service for recipient: {Email}", email);
			await _customSmtpService.SendEmailAsync(email, subject, htmlMessage);
		}
		else if (_emailSettings.Azure.IsConfigured)
		{
			_logger.LogInformation("Routing email to Azure Communication Email service for recipient: {Email}", email);
			await _azureService.SendEmailAsync(email, subject, htmlMessage);
		}
		else
		{
			_logger.LogWarning("No email provider is configured. Attempting fallback to Azure email service.");
			await _azureService.SendEmailAsync(email, subject, htmlMessage);
		}
	}

	public async Task SendBlindEmailToMultipleAsync(IEnumerable<string> emails, string subject, string htmlMessage)
	{
		if (_emailSettings.CustomSmtp.IsConfigured)
		{
			_logger.LogInformation("Routing multiple BCC email to Custom SMTP service");
			await _customSmtpService.SendBlindEmailToMultipleAsync(emails, subject, htmlMessage);
		}
		else if (_emailSettings.Azure.IsConfigured)
		{
			_logger.LogInformation("Routing multiple BCC email to Azure Communication Email service");
			await _azureService.SendBlindEmailToMultipleAsync(emails, subject, htmlMessage);
		}
		else
		{
			_logger.LogWarning("No email provider is configured. Attempting fallback to Azure email service.");
			await _azureService.SendBlindEmailToMultipleAsync(emails, subject, htmlMessage);
		}
	}
}
