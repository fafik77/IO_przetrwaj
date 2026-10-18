using Microsoft.AspNetCore.Identity;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Services;

public class SignInWithGoogleService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IUnitOfWork unitOfWork,
    IJwtService jwtService) : ISignInWithGoogleService
{
    public async Task<JwtTokenDto> SignIn(LoginWithExternalProvider info, CancellationToken cancellationToken)
    {
        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);

        AppUser? user;
        if (result.Succeeded)
        {   //User linked this Google account before
            user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        }
        else
        {
            user = await userManager.FindByEmailAsync(info.Email);
            // auto Register
            if (user is null)
            {
                user = new AppUser
                {
                    UserName = info.Email,
                    Email = info.Email,
                    EmailConfirmed = true,  // Google already verified this email
                    Name = info.Name,
                    Surname = info.Surname,
                };
                await userManager.CreateAsync(user);
            }
            else
            {   // auto confirm the email
                user.EmailConfirmed = true;
            }
            // Link the Google account to the Identity user for future Logins
            await userManager.AddLoginAsync(user, new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.LoginProvider));
            await unitOfWork.SaveChangesAsync(cancellationToken);  // make sure to save changes in DB
        }
        if (user is null) throw new UserNotFoundException(info.Email ?? string.Empty);

        UserWithPersonalDataDto userDto = (UserWithPersonalDataDto)user;
        var roles = await userManager.GetRolesAsync(user);
        userDto.Roles = roles;
        if (user.BanDate != null)   //user is banned
        {
            var bannedBy = await userManager.FindByIdAsync(user.BannedById ?? string.Empty);
            if (userDto.BanInfo != null) userDto.BanInfo.BannedBy = UserGeneralDto.Map(bannedBy);
            throw new UserBannedException("User is banned", userDto.BanInfo!);
        }

        return await jwtService.GenerateTokenAsync(userDto, cancellationToken);
    }

}
