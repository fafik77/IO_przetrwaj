using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace PrzetrwajPL.Controllers;

[Route("account")]
public class AccountController : Controller
{
	private readonly IHttpContextAccessor _ContextAccessor;
	private readonly IHttpClientFactory _ClientFactory;

	public AccountController(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
	{
		_ContextAccessor = httpContextAccessor;
		_ClientFactory = httpClientFactory;
	}

	[HttpGet("signin")]
	public async Task<IResult> Signin(string token, string? refreshToken)
	{
		if (string.IsNullOrEmpty(token))
			return Results.LocalRedirect("/");

		var handler = new JwtSecurityTokenHandler();
		if (!handler.CanReadToken(token)) return Results.BadRequest("Invalid Token");

		var jwtToken = handler.ReadJwtToken(token);

		// extract only sub for minimal claim
		var sub = jwtToken.Claims.FirstOrDefault(c => string.Equals(c.Type, "sub", StringComparison.OrdinalIgnoreCase))?.Value
				  ?? Guid.NewGuid().ToString();

		var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, sub) };
		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);

		//make the store
		var authProperties = new AuthenticationProperties
		{
			IsPersistent = true,
			ExpiresUtc = DateTimeOffset.UtcNow.AddDays(31)
		};

		//save poth the JWT and refresh token in it
		var tokensToStore = new List<AuthenticationToken>
		{
			new AuthenticationToken { Name = "access_token", Value = token }
		};
		if (!string.IsNullOrEmpty(refreshToken))
		{
			tokensToStore.Add(new AuthenticationToken { Name = "refresh_token", Value = refreshToken });
		}

		authProperties.StoreTokens(tokensToStore);

		//save it to be able to connect to user "/Account/"
		await _ContextAccessor.HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal,
			authProperties);

		//let the `RefreshCookie` method pull all data about the user
		return Results.LocalRedirect($"/account/refresh-cookie?redirectTo={Consts.RedirectLoggedInUserTo}&succesMsg=false");
	}


	[HttpGet("refresh-cookie")]
	public async Task<IResult> RefreshCookie(string redirectTo = "", bool succesMsg = true, CancellationToken ct = default)
	{
		redirectTo = redirectTo.Trim();
		if (!redirectTo.StartsWith('/')) redirectTo = "/" + redirectTo;

		var httpContext = _ContextAccessor.HttpContext;
		if (httpContext == null) return Results.BadRequest("Missing HTTP Context");

		// Important: fetch the existing authentication properties/tokens BEFORE rewriting the session !
		var authResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		// If authResult is successful, it contains the old tokens. If it fails, fallback to a safe default.
		var authProperties = authResult.Properties ?? new AuthenticationProperties
		{
			IsPersistent = true,
			ExpiresUtc = DateTimeOffset.UtcNow.AddDays(31) // 31-day lifespan
		};

		string succStr = "Zmiany zapisane pomyślnie";
		// Fetch up-to-date user information from the API
		try
		{
			var client = _ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var user = await client.GetFromJsonAsync<UserWithPersonalDataDto>("/Account", ct);
			await SaveUserAuthenticationCookie(httpContext, authProperties, user);
		}
		catch (Exception)
		{
			succStr = "Nie udało się pobrać danych z serwera";
		}

		return Results.LocalRedirect(redirectTo + (succesMsg == true ? $"?success={WebUtility.UrlEncode(succStr)}" : ""));
	}

	private static async Task SaveUserAuthenticationCookie(HttpContext httpContext, AuthenticationProperties authProperties, UserWithPersonalDataDto user)
	{
		// build the claims array with fresh data
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.Id),
			new(ClaimTypes.Name, user.Name ?? user.Email!),
			new(ClaimTypes.Email, user.Email!),
			new(ClaimNames.Region, user.Region?.Id.ToString() ?? "0"),
			new(ClaimNames.Surname, user.Surname ?? ""),
			new(ClaimNames.Name, user.Name ?? ""),
			new(ClaimNames.Impediments, user.Impediments.ToString() ?? "0"),
		};

		foreach (var role in user.Roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);

		// store the extracted authProperties (with the old tokens intact!) back
		await httpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal,
			authProperties);
	}
}
