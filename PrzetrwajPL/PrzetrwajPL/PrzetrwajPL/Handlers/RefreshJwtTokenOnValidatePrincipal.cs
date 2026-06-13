using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PrzetrwajPL.Handlers;

public static class RefreshJwtTokenOnValidatePrincipal
{
	public static async Task OnValidatePrincipal(CookieValidatePrincipalContext context)
	{
		var principal = context.Principal;
		// if user is authenticated return
		if (principal?.Identity?.IsAuthenticated != true)
			return;

		var accessToken = context.Properties.GetTokenValue("access_token");
		var refreshToken = context.Properties.GetTokenValue("refresh_token");

		if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
			return;

		// parse JWT token, to see its date
		var handler = new JwtSecurityTokenHandler();
		if (handler.ReadToken(accessToken) is not JwtSecurityToken jwtToken)
			return;

		// refresh token if it expired or will expire in 1 minute
		if (jwtToken.ValidTo > DateTime.UtcNow.AddMinutes(1))
			return;

		var clientFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
		// Build a separate refresh token, isolated client to call the refresh endpoint to prevent recursive execution loops
		var refreshClient = clientFactory.CreateClient(Consts.PrzetrwajApiRefreshClientName);
		try
		{
			var refreshPayload = new { refreshToken };
			var refreshResponse = await refreshClient.PostAsJsonAsync("Account/refresh-token", refreshPayload);

			if (refreshResponse.IsSuccessStatusCode)
			{
				var newTokens = await refreshResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
				if (newTokens is { Success: true, Token: not null })
				{
					context.Properties.StoreTokens(
					[
						new AuthenticationToken { Name = "access_token", Value = newTokens.Token },
						new AuthenticationToken { Name = "refresh_token", Value = newTokens.RefreshToken ?? refreshToken }
					]);
					// inform the ASP.NET Core, that session has changed and a new cookie was issued
					context.ShouldRenew = true;
					return;
				}
			}
			// if we are here that means the "refresh_token" is no longer valid, drop it
			context.RejectPrincipal();
			await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}
		catch (Exception)
		{
			// silence the exception, this is at the application root
		}
	}

}
