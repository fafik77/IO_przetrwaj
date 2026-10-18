using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Login;

public class GoogleLoginWithTokenCommandHandler(
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    IOptions<OAuth> oauthOptions
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

        // Find user by Google login or email
        var user = await userManager.FindByLoginAsync("Google", payload.Subject);
        if (user == null)
        {
            user = await userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                // Auto-register new user
                user = new AppUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    EmailConfirmed = true,
                    Name = payload.GivenName ?? string.Empty,
                    Surname = payload.FamilyName ?? string.Empty,
                };
                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    throw new GoogleAuthFailed(string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                user.EmailConfirmed = true;
            }
            // Link Google account for future logins
            await userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Check for bans
        UserWithPersonalDataDto userDto = (UserWithPersonalDataDto)user;
        userDto.Roles = await userManager.GetRolesAsync(user);
        if (user.BanDate != null)
        {
            var bannedBy = await userManager.FindByIdAsync(user.BannedById ?? string.Empty);
            if (userDto.BanInfo != null) userDto.BanInfo.BannedBy = UserGeneralDto.Map(bannedBy);
            throw new UserBannedException("User is banned", userDto.BanInfo!);
        }

        // Generate app JWT token
        return await jwtService.GenerateTokenAsync(userDto, cancellationToken);
    }
}