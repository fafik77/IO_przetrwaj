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

		var email = info.Principal.FindFirstValue(ClaimTypes.Email);
		var user = await _userManager.FindByEmailAsync(email);

		if (result.Succeeded && user != null)
		{
			if (user.Banned || user.BanDate != null)    //user is banned
			{
				var bannedBy = await _userManager.FindByIdAsync(user.BannedById!);
				var dtoBanned = (UserWithPersonalDataDto)user;
				if (dtoBanned.BanInfo != null) dtoBanned.BanInfo.BannedBy = (UserGeneralDto?)bannedBy!;
				throw new UserBannedException("User is banned", dtoBanned);
			}
			var dtoUser = (UserWithPersonalDataDto)user;
			var rolesUser = await _userManager.GetRolesAsync(user);
			dtoUser.Role = string.Join(", ", rolesUser);
			return dtoUser;
		}

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

		user.EmailConfirmed = true;
		// 3. Link the Google account to the Identity user for future Logins (pt. 1)
		await _userManager.AddLoginAsync(user, info);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		await _signInManager.SignInAsync(user, isPersistent: true);

		var dto = (UserWithPersonalDataDto)user;
		var roles = await _userManager.GetRolesAsync(user);
		dto.Role = string.Join(", ", roles);
		return dto;
	}
}