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
		var claims = new List<Claim>();

		// Case-insensitive matching logic on token payloads
		var sub = jwtToken.Claims.FirstOrDefault(c => string.Equals(c.Type, "sub", StringComparison.OrdinalIgnoreCase))?.Value;
		var email = jwtToken.Claims.FirstOrDefault(c => string.Equals(c.Type, "email", StringComparison.OrdinalIgnoreCase))?.Value;
		var region = jwtToken.Claims.FirstOrDefault(c => string.Equals(c.Type, "Region", StringComparison.OrdinalIgnoreCase))?.Value;
		var name = jwtToken.Claims.FirstOrDefault(c => string.Equals(c.Type, "Name", StringComparison.OrdinalIgnoreCase))?.Value;
		var surname = jwtToken.Claims.FirstOrDefault(c => string.Equals(c.Type, "Surname", StringComparison.OrdinalIgnoreCase))?.Value;
		var roles = jwtToken.Claims.Where(c => string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase));

		if (!string.IsNullOrEmpty(sub))
			claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
		if (!string.IsNullOrEmpty(email))
		{
			claims.Add(new Claim(ClaimTypes.Name, email));
			claims.Add(new Claim(ClaimTypes.Email, email));
		}
		if (!string.IsNullOrEmpty(region))
			claims.Add(new Claim("Region", region));
		if (!string.IsNullOrEmpty(name))
			claims.Add(new Claim("Name", name));
		if (!string.IsNullOrEmpty(surname))
			claims.Add(new Claim("Surname", surname));

		if (roles.Any())
		{
			foreach (var r in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, r.Value));
			}
		}
		else
		{
			claims.Add(new Claim(ClaimTypes.Role, "User"));
		}

		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);

		// Authentication kept for 31 days
		var authProperties = new AuthenticationProperties
		{
			IsPersistent = true,
			ExpiresUtc = DateTimeOffset.UtcNow.AddDays(31)
		};

		// Bundle both tokens securely into the Cookie container
		var tokensToStore = new List<AuthenticationToken>
		{
			new AuthenticationToken { Name = "access_token", Value = token }
		};

		if (!string.IsNullOrEmpty(refreshToken))
		{
			tokensToStore.Add(new AuthenticationToken { Name = "refresh_token", Value = refreshToken });
		}

		authProperties.StoreTokens(tokensToStore);

		await _ContextAccessor.HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal,
			authProperties);

		return Results.LocalRedirect("/");
	}

	[HttpGet("refresh-cookie")]
	public async Task<IResult> RefreshCookie(string redirectTo = "", CancellationToken ct = default)
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

		// Fetch up-to-date user information from the API
		var client = _ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
		var user = await client.GetFromJsonAsync<UserWithPersonalDataDto>("/Account", ct);

		// Rebuild the claims array with fresh data
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id),
			new Claim(ClaimTypes.Name, user.Name ?? user.Email!),
			new Claim(ClaimTypes.Email, user.Email!),
			new Claim("Region", user.Region?.Id.ToString() ?? "0"),
			new Claim("Surname", user.Surname ?? ""),
			new Claim("Name", user.Name ?? ""),
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

		var succStr = "Zmiany zapisane pomyślnie";
		return Results.LocalRedirect(redirectTo + $"?success={WebUtility.UrlEncode(succStr)}");
	}
}
