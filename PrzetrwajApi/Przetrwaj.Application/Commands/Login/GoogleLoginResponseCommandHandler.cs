using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Models.Dtos;
using System.Security.Claims;

namespace Przetrwaj.Application.Commands.Login;

public class GoogleLoginResponseCommandHandler : ICommandHandler<GoogleLoginResponseCommand, UserWithPersonalDataDto>
{
	private readonly UserManager<AppUser> _userManager;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly IUnitOfWork _unitOfWork;

	public GoogleLoginResponseCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUnitOfWork unitOfWork)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_unitOfWork = unitOfWork;
	}

	public async Task<UserWithPersonalDataDto> Handle(GoogleLoginResponseCommand request, CancellationToken cancellationToken)
	{
		var info = await _signInManager.GetExternalLoginInfoAsync();
		if (info is null) throw new GoogleAuthFailed("GoogleAuthFailed");

		// 1. Try to sign in the user if they've linked this Google account before
		var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);

		//var email = info.Principal.FindFirstValue(ClaimTypes.Email);
		//var user = await _userManager.FindByEmailAsync(email);
		AppUser? user;
		if (result.Succeeded)
		{
			user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
		}
		else
		{
			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			user = await _userManager.FindByEmailAsync(email!);
			// 2. If user doesn't exist, create them (auto Register)
			if (user is null)
			{
				user = new AppUser
				{
					UserName = email,
					Email = email,
					EmailConfirmed = true, // Google already verified this email
					Name = info.Principal.FindFirstValue(ClaimTypes.GivenName),
					Surname = info.Principal.FindFirstValue(ClaimTypes.Surname)
				};
				await _userManager.CreateAsync(user);
			}
			else
			{	//confirm the email
				user.EmailConfirmed = true;
			}
			// 3. Link the Google account to the Identity user for future Logins (pt. 1)
			await _userManager.AddLoginAsync(user, info);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			await _signInManager.SignOutAsync(); // Clear External Cookie
			await _signInManager.SignInAsync(user, isPersistent: true);
		}

		var dtoUser = (UserWithPersonalDataDto)user;
		var rolesUser = await _userManager.GetRolesAsync(user);
		dtoUser.Role = string.Join(", ", rolesUser);
		await _signInManager.SignInAsync(user, isPersistent: true);
		if (user.Banned || user.BanDate != null)    //user is banned
		{
			var bannedBy = await _userManager.FindByIdAsync(user.BannedById!);
			if (dtoUser.BanInfo != null) dtoUser.BanInfo.BannedBy = (UserGeneralDto?)bannedBy!;
			throw new UserBannedException("User is banned", dtoUser);
		}
		return dtoUser;
	}
}