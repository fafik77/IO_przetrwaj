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

	public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IOptions<FrontEndSettings> frontEndSettings)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_emailSender = emailSender;
		_urlHelper = urlHelper;
		_httpContextAccessor = httpContextAccessor;
		_userRepository = userRepository;
		_frontEndSettings = frontEndSettings.Value;
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

	public async Task GenerateChangeEmailTokenAsync(AppUser user, string newEmail)
	{
		var ChangeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
		ConfirmEmailChangeInfo values = new() { UserId = user.Id, Code = ChangeEmailToken, NewEmail = newEmail };
		string absoluteUrlString = GenerateEmailConfirmationUrl(action: "ConfirmEmailChange", values);
		// send the email
		await _emailSender.SendEmailAsync(newEmail, subject: "Potwierdź zmianę swojego adresu e-mail. Przetrwaj.pl",
			$"<h2>{user.Name} właśnie chcesz zmienić swój adres e-mail w serwisie <a href='{_frontEndSettings.Url}'>Przetrwaj.pl</a> na {newEmail}</h2><br>" +
			$"Potwierdź zmianę swojego adresu e-mail, <a href='{(absoluteUrlString)}'>klikając tutaj</a>." +
			$"<br>Dopóki tego nie zrobisz, do serwisu będziesz logować się obecnym e-mailem." +
			$"<br><br><p style='color: gray; font-size: 12px;'>Ten email został wysłany automatycznie z serwisu <a href='{_frontEndSettings.Url}'>Przetrwaj.pl</a> prosimy na niego nie odpowiadać.</p>");
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

		if (user.Banned || user.BanDate != null)    //user is banned
		{
			var bannedBy = await _userRepository.GetByIdAsync(user.BannedById!);
			var dto = (UserWithPersonalDataDto)user;
			dto.BannedBy = (UserGeneralDto?)bannedBy!;
			throw new UserBannedException("User is banned", dto);
		}
		var signedIn = await _signInManager.PasswordSignInAsync(user, password, true, true);
		if (signedIn.Succeeded)
			return user;
		throw new InvalidLoginException("Bad login attempt");
	}

	public async Task<AppUser> RegisterUserByEmailAsync(RegisterEmailInfo register)
	{
		var user = new AppUser
		{
			Email = register.Email,
			Name = register.Name,
			Surname = register.Surname,
			UserName = register.Email, // Typically, UserName is set to the email for login (its enforced unique)
			IdRegion = register.IdRegion ?? 0,
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
		string absoluteUrlString = GenerateEmailConfirmationUrl(action: "ConfirmEmail", values);
		// send the email
		await _emailSender.SendEmailAsync(register.Email, subject: "Potwierdź swój adres e-mail. Przetrwaj.pl",
			$"<h2>{register.Name} witaj w serwisie <a href='{_frontEndSettings.Url}'>Przetrwaj.pl</a></h2><br>" +
			$"Potwierdź swoje konto, <a href='{(absoluteUrlString)}'>klikając tutaj</a>." +
			$"<br><br><p style='color: gray; font-size: 12px;'>Ten email został wysłany automatycznie z serwisu <a href='{_frontEndSettings.Url}'>Przetrwaj.pl</a> prosimy na niego nie odpowiadać.</p>");

		return user;
	}

	private string GenerateEmailConfirmationUrl(string action, object values)
	{
		// 2. Generate the relative URL path using IUrlHelper.Action()
		// This is equivalent to your old 'Url.Action' call
		var relativeUrl = _urlHelper.Action(
			action: action,
			controller: "Account",
			values: values);
		// Check if relativeUrl is null (route not found)
		if (string.IsNullOrEmpty(relativeUrl))
		{   //Email confirmation related errors
			throw new InvalidConfirmationException("Could not generate confirmation URL.");
		}
		// 3. Get the request scheme and host from HttpContext
		var request = _httpContextAccessor.HttpContext?.Request;
		if (request == null)
		{   //Email confirmation related errors
			throw new InvalidConfirmationException("Cannot access HTTP context to build URL.");
		}
		var scheme = request.Scheme;
		var host = request.Host.Value;
		// 4. Construct the absolute URL string
		// e.g., "localhost:5001" or "api.example.com"
		var absoluteUrlString = $"{scheme}://{host}{relativeUrl}";
		return absoluteUrlString;
	}
}
