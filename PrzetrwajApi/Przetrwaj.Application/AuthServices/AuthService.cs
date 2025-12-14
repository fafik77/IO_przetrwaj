using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models;

namespace Przetrwaj.Application.AuthServices;

public class AuthService : IAuthService
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IUserRepository _userRepository;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly IEmailSender _emailSender;
	private readonly IUrlHelper _urlHelper;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_emailSender = emailSender;
		_urlHelper = urlHelper;
		_httpContextAccessor = httpContextAccessor;
		_userRepository = userRepository;
	}


	public async Task<AppUser> ConfirmEmailAsync(string userId, string code)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			throw new InvalidDataException("User not found.");
		var result = await _userManager.ConfirmEmailAsync(user, code);

		if (result.Succeeded)
			return user;

		throw new InvalidDataException("Email confirmation failed.");
	}

	public async Task<AppUser?> GetUserDetailsAsync(string userIdEmail)
	{
		AppUser? user;
		if (userIdEmail.Contains("@"))
			user = await _userRepository.GetByEmailAsync(userIdEmail);
		else
			user = await _userRepository.GetByIdAsync(userIdEmail);
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
			return user;

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
		};

		var result = await _userManager.CreateAsync(user, register.Password);
		if (!result.Succeeded)
		{   // do not expose too much info
			string errors = string.Join("\n", result.Errors.Where(e => e.Code.Contains("Password", StringComparison.OrdinalIgnoreCase)).Select(e => e.Description).ToList());
			if (string.IsNullOrEmpty(errors))
				throw new RegisterException($"Could not register email: {register.Email} with password: {register.Password}.\nTry another email or password");
			throw new RegisterException(errors);
		}

		// 1. Generate the Code
		var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		// 2. Generate the relative URL path using IUrlHelper.Action()
		// This is equivalent to your old 'Url.Action' call
		var relativeUrl = _urlHelper.Action(
			action: "ConfirmEmail",
			controller: "Account",
			values: new ConfirmEmailInfo { userId = user.Id, code = code });
		// Check if relativeUrl is null (route not found)
		if (string.IsNullOrEmpty(relativeUrl))
		{   //Email confirmation related errors
			throw new RegisterException("Could not generate confirmation URL.");
		}
		// 3. Get the request scheme and host from HttpContext
		var request = _httpContextAccessor.HttpContext?.Request;
		if (request == null)
		{   //Email confirmation related errors
			throw new RegisterException("Cannot access HTTP context to build URL.");
		}
		var scheme = request.Scheme;
		var host = request.Host.Value;
		// 4. Construct the absolute URL string
		// e.g., "localhost:5001" or "api.example.com"
		var absoluteUrlString = $"{scheme}://{host}{relativeUrl}";
		// send the email
		await _emailSender.SendEmailAsync(register.Email, "Confirm your email",
			$"Please confirm your account by <a href='{(absoluteUrlString)}'>clicking here</a>.");

		return user;
	}
}
