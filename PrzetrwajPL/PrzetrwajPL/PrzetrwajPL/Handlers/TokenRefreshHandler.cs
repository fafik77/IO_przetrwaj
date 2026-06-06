namespace PrzetrwajPL.Handlers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Przetrwaj.CommonLibrary.Consts;
using PrzetrwajPL.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class TokenRefreshHandler : DelegatingHandler
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IHttpClientFactory _httpClientFactory;

	public TokenRefreshHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
	{
		_httpContextAccessor = httpContextAccessor;
		_httpClientFactory = httpClientFactory;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		// Try to proceed with the primary outbound request
		var response = await base.SendAsync(request, cancellationToken);

		// API returned a 401 (Unauthorized), send a refresh token
		if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			var context = _httpContextAccessor.HttpContext;
			if (context == null) return response;

			// Retrieve the refresh token, securely stored inside the encrypted Cookie state
			var refreshToken = await context.GetTokenAsync("refresh_token");
			if (string.IsNullOrEmpty(refreshToken)) return response;

			// Build a clean, isolated client to call the refresh endpoint to prevent recursive execution loops
			var refreshClient = _httpClientFactory.CreateClient();
			refreshClient.BaseAddress = _httpClientFactory.CreateClient(Consts.PrzetrwajApiClientName).BaseAddress;

			// Prepare payload matching: {"refreshToken": "string"}
			var refreshPayload = new { refreshToken };

			var refreshResponse = await refreshClient.PostAsJsonAsync("Account/refresh-token", refreshPayload, cancellationToken);

			if (refreshResponse.IsSuccessStatusCode)
			{
				var newTokens = await refreshResponse.Content.ReadFromJsonAsync<JwtTokenDto>(cancellationToken: cancellationToken);

				if (newTokens is { Success: true, Token: not null })
				{
					// 5. Update the failing request's header with the fresh Bearer Token
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newTokens.Token);

					// 6. Refresh the local underlying authentication cookie values
					var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

					if (authResult.Principal != null)
					{
						var properties = authResult.Properties ?? new AuthenticationProperties();
						properties.StoreTokens(new[]
						{
							new AuthenticationToken { Name = "access_token", Value = newTokens.Token },
							new AuthenticationToken { Name = "refresh_token", Value = newTokens.RefreshToken ?? refreshToken }
						});

						// Re-sign the identity context to persist the new tokens to the local memory layer
						await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authResult.Principal, properties);
					}

					// 7. Re-fire the original request using the updated token reference
					response.Dispose(); // Clean up the original 401 response payload
					return await base.SendAsync(request, cancellationToken);
				}
			}
		}

		return response;
	}
}
