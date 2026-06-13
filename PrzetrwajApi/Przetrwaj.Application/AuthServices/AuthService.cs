using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Helpers;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.AuthServices;

public class AuthService : IAuthService
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IUserRepository _userRepository;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly IEmailSender _emailSender;
	private readonly IUrlHelper _urlHelper;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly FrontEndSettings _frontEndSettings;
	private readonly IRegionRepository _regionRepository;

	public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IOptions<FrontEndSettings> frontEndSettings, IRegionRepository regionRepository)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_emailSender = emailSender;
		_urlHelper = urlHelper;
		_httpContextAccessor = httpContextAccessor;
		_userRepository = userRepository;
		_frontEndSettings = frontEndSettings.Value;
		_regionRepository = regionRepository;
	}


	public async Task<AppUser> ConfirmEmailAsync(string userId, string code)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			throw new InvalidConfirmationException("User not found.");
		var result = await _userManager.ConfirmEmailAsync(user, code);

		if (result.Succeeded)
			return user;

		throw new InvalidConfirmationException("Email confirmation failed.");
	}

	public async Task GenerateChangeEmailTokenAsync(AppUser user, string newEmail, string? returnUrl)
	{
		// 1. Generate the Code
		var ChangeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
		ConfirmEmailChangeInfo values = new() { UserId = user.Id, Code = ChangeEmailToken, NewEmail = newEmail };
		string absoluteUrlString = GenerateEmailConfirmationUrl(action: "confirm-email-change", values, returnUrl);
		// send the email
		await _emailSender.SendEmailAsync(newEmail, subject: "Potwierdź zmianę adresu e-mail - Przetrwaj.pl",
		$@"<h2>Witaj {user.Name}!</h2>
		<p>Otrzymaliśmy prośbę o zmianę adresu e-mail na: <strong>{newEmail}</strong>.</p>
		<div style='margin: 30px 0;'>
			<a href='{absoluteUrlString}' 
			   style='background-color: #007bff; color: white; padding: 15px 25px; text-decoration: none; font-size: 18px; border-radius: 5px; display: inline-block;'>
			   Potwierdź zmianę e-maila
			</a>
		</div>
		<p>Dopóki nie klikniesz w przycisk, Twój obecny adres pozostanie aktywny.</p>");
	}

	public async Task<AppUser> GetUserDetailsAsync(string userIdEmail)
	{
		AppUser? user;
		if (userIdEmail.Contains('@'))
			user = await _userRepository.GetByEmailAsync(userIdEmail);
		else
			user = await _userRepository.GetByIdAsync(userIdEmail);
		if (user is null) throw new UserNotFoundException(userIdEmail);
		return user;
	}

	public async Task<AppUser> LoginUserByEmailAsync(string email, string password)
	{
		//var user = await _userManager.FindByEmailAsync(email);
		var user = await _userRepository.GetByEmailAsync(email);
		if (user == null || user.EmailConfirmed == false)
			throw new InvalidLoginException("Bad login attempt");

		if (await _userManager.IsLockedOutAsync(user))
			throw new InvalidLoginException("Bad login attempt");

		if (false == await _userManager.CheckPasswordAsync(user, password))
			throw new InvalidLoginException("Bad login attempt");

		if (user.BanDate != null)    //user is banned
		{
			var bannedBy = await _userRepository.GetByIdAsync(user.BannedById ?? string.Empty);
			UserWithPersonalDataDto dto = (UserWithPersonalDataDto)user;
			if (dto.BanInfo != null) dto.BanInfo.BannedBy = UserGeneralDto.Map(bannedBy);
			throw new UserBannedException("User is banned", dto.BanInfo!);
		}
		var signedIn = await _signInManager.PasswordSignInAsync(user, password, true, true);
		if (signedIn.Succeeded)
			return user;
		throw new InvalidLoginException("Bad login attempt");
	}

	public async Task<AppUser> RegisterUserByEmailAsync(RegisterEmailInfo register, string? returnUrl)
	{
		var (Woj, Pow, Gmi) = RegionCompoundHelper.RegionSplit(register.IdRegion);
		var GmiExists = await _regionRepository.GetByIdAsync(Gmi);
		var user = new AppUser
		{
			Email = register.Email,
			Name = register.Name,
			Surname = register.Surname,
			UserName = register.Email, // Typically, UserName is set to the email for login (its enforced unique)
			GminaId = GmiExists is null ? null : Gmi,
			RegistrationDate = DateTimeOffset.UtcNow,
			ModeratorRolePending = register.ModeratorRole,
		};

		var result = await _userManager.CreateAsync(user, register.Password);
		if (!result.Succeeded)
		{   // do not expose too much info
			string errors = string.Join("\n", result.Errors
				.Where(e => e.Code.Contains("Password", StringComparison.OrdinalIgnoreCase)
					|| e.Code.Contains("DuplicateEmail", StringComparison.OrdinalIgnoreCase))
				.Select(e => e.Description).ToList());
			if (string.IsNullOrEmpty(errors))
				throw new RegisterException($"Could not register email: {register.Email} with password: {register.Password}.\nTry another email or password");
			throw new RegisterException(errors);
		}

		// 1. Generate the Code
		var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		ConfirmEmailInfo values = new() { UserId = user.Id, Code = code };
		string absoluteUrlString = GenerateEmailConfirmationUrl(action: "confirm-email", values, returnUrl);
		// send the email
		await _emailSender.SendEmailAsync(register.Email, subject: "Potwierdź swój adres e-mail. Przetrwaj.pl",
			$"<h2>{register.Name} witaj w serwisie Przetrwaj.pl</h2><br>" +
			$@"<p>Potwierdź swoje konto, klikając w poniższy przycisk:</p>
<div style='text-align: center; margin: 30px 0;'>
	<a href='{absoluteUrlString}' 
	   style='background-color: #28a745; 
			  color: white; 
			  padding: 15px 25px; 
			  text-decoration: none; 
			  font-size: 18px; 
			  font-weight: bold; 
			  border-radius: 5px; 
			  display: inline-block;'>
		Potwierdź konto
	</a>
</div>" +
			$"<br><br><p style='color: gray; font-size: 12px;'>Ten email został wysłany automatycznie z serwisu <a href='{_frontEndSettings.Url}'>Przetrwaj.pl</a> prosimy na niego nie odpowiadać.</p>");

		return user;
	}

	// Helper to prevent Open Redirect attacks
	private bool IsUrlSafe(string url)
	{
		// Check if it starts with front-end domain or is a local path
		return url.StartsWith(_frontEndSettings.Url) || url.StartsWith("https://localhost:");
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="action">the action path in AccountController (not the method name!)</param>
	/// <param name="values"></param>
	/// <param name="returnUrl">if present and safe it will link to it</param>
	/// <returns>URL with encoded query params</returns>
	/// <exception cref="InvalidConfirmationException"></exception>
	private string GenerateEmailConfirmationUrl(string action, ConfirmEmailInfo values, string? returnUrl)
	{
		if (string.IsNullOrEmpty(returnUrl) || !IsUrlSafe(returnUrl))
			returnUrl = _frontEndSettings.Url; // Fallback to frontEnd Url

		if (string.IsNullOrEmpty(returnUrl))
		{
			// we could not fetch any frontend Url. hadle it with backend only
			var request = _httpContextAccessor.HttpContext?.Request;
			var scheme = request?.Scheme;
			var host = request?.Host.Value;
			// the absolute URL string
			// e.g., "https://" +("localhost:5001" or "api.example.com")
			returnUrl = $"{scheme}://{host}";
			if (!returnUrl.EndsWith('/')) returnUrl += "/";
			returnUrl += "Account"; //the path to AccountController
		}

		if (!returnUrl.EndsWith('/')) returnUrl += "/";
		var baseUrl = returnUrl + action;

		return values.ToQueryString(baseUrl);
	}
}
