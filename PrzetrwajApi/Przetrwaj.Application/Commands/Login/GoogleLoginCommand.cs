using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Login;

public class GoogleLoginCommand(string redirectUrl) : ICommand<Microsoft.AspNetCore.Authentication.AuthenticationProperties>
{
	[Required]
	public string RedirectUrl { get; set; } = redirectUrl;
}
