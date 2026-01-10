using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Settings;

namespace Przetrwaj.Application.Services;

public class EmailAzureService : IEmailSenderMultiple
{
	private EmailSettings AzureEmailSettings { get; }
	private EmailClient? EmailClient { get; } = null;

	public EmailAzureService(IOptions<EmailSettings> options)
	{
		AzureEmailSettings = options.Value;
		if (string.IsNullOrEmpty(AzureEmailSettings.AzureConnection))
			return; //do not throw an error at this point, we only injected it
		EmailClient = new EmailClient(AzureEmailSettings.AzureConnection);
	}

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var emailMessage = new EmailMessage(
			senderAddress: AzureEmailSettings.AzureSender,
			content: new EmailContent(subject)
			{
				PlainText = htmlMessage,
				Html = htmlMessage
			},
			recipients: new EmailRecipients(new List<EmailAddress>
			{
					new EmailAddress(email)
			}));

		if (EmailClient == null)
			throw new NotImplementedException("Email client not configured!");

		EmailSendOperation emailSendOperation = await EmailClient.SendAsync(
			WaitUntil.Completed,
			emailMessage);
	}

	public async Task SendBlindEmailToMultipleAsync(IEnumerable<string> emails, string subject, string htmlMessage)
	{
		if (EmailClient == null) throw new NotImplementedException("Email client not configured.");

		var recipients = emails.Select(e => new EmailAddress(e)).ToList();

		var emailMessage = new EmailMessage(
			senderAddress: AzureEmailSettings.AzureSender,
			content: new EmailContent(subject) { PlainText = htmlMessage, Html = htmlMessage },
			recipients: new EmailRecipients(bcc: recipients)); // Use BCC(Blind Carbon Copy) here. Recipients do not see each other

		await EmailClient.SendAsync(WaitUntil.Started, emailMessage);
	}
}
