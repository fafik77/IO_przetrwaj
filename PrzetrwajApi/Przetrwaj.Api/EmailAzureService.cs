using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace Przetrwaj.Api;

public class EmailAzureService : IEmailSender
{
	public EmailAzureService(IOptions<EmailSettings> options)
	{
		AzureEmailSettings = options.Value;
		if (string.IsNullOrEmpty(AzureEmailSettings.AzureConnection))
			return; //do not throw an error at this point, we only injected it
		this.EmailClient = new EmailClient(AzureEmailSettings.AzureConnection);
	}

	private EmailSettings AzureEmailSettings { get; }
	private EmailClient? EmailClient { get; } = null;

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
			throw new NotImplementedException("Send Email is not configured!");

		EmailSendOperation emailSendOperation = await EmailClient.SendAsync(
			WaitUntil.Completed,
			emailMessage);
	}
}
