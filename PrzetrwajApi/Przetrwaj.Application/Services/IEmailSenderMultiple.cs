using Microsoft.AspNetCore.Identity.UI.Services;

namespace Przetrwaj.Application.Services;

public interface IEmailSenderMultiple : IEmailSender
{
	public Task SendBlindEmailToMultipleAsync(IEnumerable<string> emails, string subject, string htmlMessage);
}
