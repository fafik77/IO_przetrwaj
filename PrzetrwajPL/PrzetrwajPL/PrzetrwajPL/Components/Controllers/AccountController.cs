using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PrzetrwajPL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace PrzetrwajPL.Components.Controllers;

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
	public async Task<IResult> Signin(string token)
	{
		if (string.IsNullOrEmpty(token))
			return Results.LocalRedirect("/");

		var handler = new JwtSecurityTokenHandler();
		if (!handler.CanReadToken(token)) return Results.BadRequest("Invalid Token");

		var jwtToken = handler.ReadJwtToken(token);

		// Extract claims exactly like your LoginUser method does
		var claims = new List<Claim>();

		// JWT standard claims to Identity claims mapping
		var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
		var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
		var region = jwtToken.Claims.FirstOrDefault(c => c.Type == "Region")?.Value;
		var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
		var surname = jwtToken.Claims.FirstOrDefault(c => c.Type == "Surname")?.Value;
		var roles = jwtToken.Claims.Where(c => c.Type == "role");

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

		if (roles.Count() != 0)
		{
			foreach (var r in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, r.Value));
			}
		}
		else
		{
			claims.Add(new Claim(ClaimTypes.Role, "User")); // Default fallback
		}

		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);

		var authProperties = new AuthenticationProperties
		{
			IsPersistent = true,
			ExpiresUtc = jwtToken.ValidTo
		};
		// Store the actual string token so we can retrieve it later
		authProperties.StoreTokens(new[]
		{
			new AuthenticationToken { Name = "access_token", Value = token }
		});
		await _ContextAccessor.HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal,
			authProperties);
		//await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
		//	new AuthenticationProperties
		//	{
		//		IsPersistent = true,
		//		ExpiresUtc = jwtToken.ValidTo
		//	});
		return Results.LocalRedirect("/");
	}

	[HttpGet("refresh-cookie")]
	public async Task<IResult> RefreshCookie(string redirectTo = "")
	{
		redirectTo = redirectTo.Trim();
		if (!redirectTo.StartsWith("/")) redirectTo = "/" + redirectTo;

		var client = _ClientFactory.CreateClient("ServerAPI"); 
		var user = await client.GetFromJsonAsync<UserWithPersonalDataDto>("/Account");

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
		await _ContextAccessor.HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal,
			new AuthenticationProperties { IsPersistent = true });
		var succStr = "Zmiany zapisane pomyślnie";
		return Results.LocalRedirect(redirectTo+$"?success={WebUtility.UrlEncode(succStr)}");
	}
}
