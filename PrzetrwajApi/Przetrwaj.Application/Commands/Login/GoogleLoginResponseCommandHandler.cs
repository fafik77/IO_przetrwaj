using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Services;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;
using System.Security.Claims;

namespace Przetrwaj.Application.Commands.Login;

public class GoogleLoginResponseCommandHandler(
     SignInManager<AppUser> signInManager,
     ISignInWithGoogleService signInWithGoogle) : ICommandHandler<GoogleLoginResponseCommand, JwtTokenDto>
{
    public async Task<JwtTokenDto> Handle(GoogleLoginResponseCommand request, CancellationToken cancellationToken)
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null) throw new GoogleAuthFailed("GoogleAuthFailed");

        LoginWithExternalProvider loginInfo = new()
        {
            Email = info.Principal.FindFirstValue(ClaimTypes.Email)!,
            Name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
            Surname = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
            Password = string.Empty,
            LoginProvider = info.LoginProvider,
            ProviderKey = info.ProviderKey
        };
        await signInManager.SignOutAsync();    // Clear External Cookie from Google auth
        return await signInWithGoogle.SignIn(loginInfo, cancellationToken);
    }
}