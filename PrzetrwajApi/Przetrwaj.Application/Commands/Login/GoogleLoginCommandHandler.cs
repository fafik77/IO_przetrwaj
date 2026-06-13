using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Login;

public class GoogleLoginCommandHandler : ICommandHandler<GoogleLoginCommand, AuthenticationProperties>
{
	private readonly SignInManager<AppUser> _signInManager;

	public GoogleLoginCommandHandler(SignInManager<AppUser> signInManager)
	{
		_signInManager = signInManager;
	}

	async Task<AuthenticationProperties> IRequestHandler<GoogleLoginCommand, AuthenticationProperties>.Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
	{
		AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", request.RedirectUrl);
		return properties;
	}
}