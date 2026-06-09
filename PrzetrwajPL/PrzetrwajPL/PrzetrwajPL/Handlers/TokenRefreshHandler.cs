namespace PrzetrwajPL.Handlers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class TokenRefreshHandler(
	IHttpContextAccessor httpContextAccessor,
	IHttpClientFactory httpClientFactory,
	ILogger<TokenRefreshHandler> logger
	) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		HttpResponseMessage response;
		try
		{
			// Try to proceed with the primary outbound request
			response = await base.SendAsync(request, cancellationToken);
		}
		catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is TimeoutException)
		{
			logger.LogError(ex, "API Server is unreachable or timed out during the initial request to {Url}", request.RequestUri);
			return CreateErrorResponse(request, HttpStatusCode.ServiceUnavailable, "Serwer nie odpowiada. Spróbuj ponownie później.");
		}


		// API returned a 401 (Unauthorized), send a refresh token
		if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			var context = httpContextAccessor.HttpContext;
			if (context == null) return response;

			// Retrieve the refresh token, securely stored inside the encrypted Cookie state
			var refreshToken = await context.GetTokenAsync("refresh_token");
			if (string.IsNullOrEmpty(refreshToken)) return response;

			try
			{
				// Build a clean, isolated client to call the refresh endpoint to prevent recursive execution loops
				var refreshClient = httpClientFactory.CreateClient();
				refreshClient.BaseAddress = httpClientFactory.CreateClient(Consts.PrzetrwajApiClientName).BaseAddress;

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
						try
						{
							return await base.SendAsync(request, cancellationToken);
						}
						catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is TimeoutException)
						{
							logger.LogError(ex, "API Server dropped connection during the retried request to {Url}", request.RequestUri);
							return CreateErrorResponse(request, HttpStatusCode.ServiceUnavailable, "Połączenie przerwane podczas ponownej próby.");
						}
					}

				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Critical failure during the token refresh execution loop.");
			}
		}

		return response;
	}

	// Helper to generate a clean, mock HTTP response without throwing system exceptions
	private static HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, HttpStatusCode statusCode, string message)
	{
		return new HttpResponseMessage(statusCode)
		{
			RequestMessage = request,
			Content = new StringContent(message)
		};
	}
}
