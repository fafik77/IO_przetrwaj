using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;

namespace PrzetrwajPL.Components.Controllers;

[Route("account")]
public class AccountController : Controller
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public AccountController(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
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
		await _httpContextAccessor.HttpContext.SignInAsync(
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
}
