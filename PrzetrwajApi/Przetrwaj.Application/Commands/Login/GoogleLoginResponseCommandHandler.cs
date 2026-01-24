using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;
using System.Security.Claims;

namespace Przetrwaj.Application.Commands.Login;

public class GoogleLoginResponseCommandHandler : ICommandHandler<GoogleLoginResponseCommand, JwtTokenDto>
{
	private readonly UserManager<AppUser> _userManager;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IJwtService _jwtService;

	public GoogleLoginResponseCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUnitOfWork unitOfWork, IJwtService jwtService)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_unitOfWork = unitOfWork;
		_jwtService = jwtService;
	}

	public async Task<JwtTokenDto> Handle(GoogleLoginResponseCommand request, CancellationToken cancellationToken)
	{
		var info = await _signInManager.GetExternalLoginInfoAsync();
		if (info is null) throw new GoogleAuthFailed("GoogleAuthFailed");

		// 1. Try to sign in the user if they've linked this Google account before
		var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);

		AppUser? user;
		if (result.Succeeded)
		{   //User linked this Google account before
			user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
		}
		else
		{   //this is the first time for this Google account
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
					Surname = info.Principal.FindFirstValue(ClaimTypes.Surname),
					IdRegion = 0,
				};
				await _userManager.CreateAsync(user);
			}
			else
			{   //confirm the email
				user.EmailConfirmed = true;
			}
			// 3. Link the Google account to the Identity user for future Logins (pt. 1)
			await _userManager.AddLoginAsync(user, info);
			await _unitOfWork.SaveChangesAsync(cancellationToken); //make sure to save changes in DB
		}
		await _signInManager.SignOutAsync(); // Clear External Cookie from Google auth

		var userDto = (UserWithPersonalDataDto)user;
		var roles = await _userManager.GetRolesAsync(user);
		userDto.Roles = roles;
		if (user.Banned || user.BanDate != null)    //user is banned
		{
			var bannedBy = await _userManager.FindByIdAsync(user.BannedById!);
			if (userDto.BanInfo != null) userDto.BanInfo.BannedBy = (UserGeneralDto?)bannedBy!;
			throw new UserBannedException("User is banned", userDto.BanInfo);
		}
		else
			await _signInManager.SignInAsync(user, isPersistent: true);

		return new JwtTokenDto
		{
			Token = _jwtService.GenerateToken(userDto)
		};
	}
}