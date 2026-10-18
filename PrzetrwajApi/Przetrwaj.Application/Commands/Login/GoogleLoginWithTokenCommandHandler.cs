using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Services;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Login;

public class GoogleLoginWithTokenCommandHandler(
    IOptions<OAuth> oauthOptions,
    ISignInWithGoogleService signInWithGoogle
    ) : ICommandHandler<GoogleLoginWithTokenCommand, JwtTokenDto>
{
    private readonly OAuth _oauthSettings = oauthOptions.Value;

    public async Task<JwtTokenDto> Handle(GoogleLoginWithTokenCommand request, CancellationToken cancellationToken)
    {
        // Verify Google ID token
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _oauthSettings.Google?.ClientId }
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch (InvalidJwtException)
        {
            throw new GoogleAuthFailed("Invalid Google token");
        }

        LoginWithExternalProvider loginInfo = new()
        {
            Email = payload.Email,
            Name = payload.GivenName,
            Surname = payload.FamilyName,
            Password = string.Empty,
            LoginProvider = "Google",
            ProviderKey = payload.Subject
        };
        return await signInWithGoogle.SignIn(loginInfo, cancellationToken);
    }
}