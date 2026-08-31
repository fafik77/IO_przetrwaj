using Microsoft.AspNetCore.Identity.UI.Services;

namespace Przetrwaj.Application.Services;

/// <summary>
/// Extends IEmailSender with SendToMultiple
/// </summary>
public interface IEmailSenderMultiple : IEmailSender
{
	/// <summary>
	/// Sends email to multiple recipients without including their addresses in TO field
	/// </summary>
	/// <param name="emails">recipients</param>
	/// <param name="subject">subject</param>
	/// <param name="htmlMessage">the message to send</param>
	public Task SendBlindEmailToMultipleAsync(IEnumerable<string> emails, string subject, string htmlMessage);
}
