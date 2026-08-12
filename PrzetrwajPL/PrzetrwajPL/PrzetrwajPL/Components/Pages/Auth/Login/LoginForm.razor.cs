using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Requests;


namespace PrzetrwajPL.Components.Pages.Auth.Login
{
	public partial class LoginForm
	{
		[CascadingParameter]
		private HttpContext? HttpContext { get; set; }

		[SupplyParameterFromForm]
		public LoginRequest LoginRequest { get; set; } = new LoginRequest();
		private string errorMessage = string.Empty;
		private bool isLoading = false;

		private string GoogleLoginUrl
		{
			get
			{
				var apiPath = $"{ClientFactory.CreateClient(Consts.PrzetrwajApiClientName).BaseAddress}Login/google";
				var currentUri = NavigationManager.BaseUri;
				// Encode the URI so it can be passed safely in a query string
				return $"{apiPath}?returnUrl={Uri.EscapeDataString(currentUri)}";
			}
		}

		private async Task HandleLogin()
		{
			isLoading = true;
			errorMessage = string.Empty;
			try
			{
				var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
				var response = await client.PostAsJsonAsync("/Login/email", LoginRequest);
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<JwtTokenDto>();
					// Gather all parameters securely to prevent URL corruption when passing parrameters
					var queryParams = new Dictionary<string, string?>
					{
						{ "token", result.Token },
						{ "refreshToken", result.RefreshToken }
					};

					var redirectUrl = QueryHelpers.AddQueryString("/account/signin", queryParams);

					// Safe execution checking for SSR vs Interactive Server Circuit
					if (HttpContext?.Response != null)
					{
						HttpContext.Response.Redirect(redirectUrl);
					}
					else
					{
						// Force a true page load so the AccountController can establish a standard HTTP Cookie context
						NavigationManager.NavigateTo(redirectUrl, forceLoad: true);
					}
				}
				else if (response.StatusCode == (System.Net.HttpStatusCode)StatusCodes.Status418ImATeapot)
				{
					var banData = await response.Content.ReadFromJsonAsync<BanInfo>();

					if (banData != null)
					{
						string jsonString = System.Text.Json.JsonSerializer.Serialize(banData);
						string secureQueryParam = Uri.EscapeDataString(jsonString);
						NavigationManager.NavigateTo($"/login-error/Banned?Info={secureQueryParam}");
					}
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
				{
					var errorText = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
					errorMessage = "Nieprawidłowy email lub hasło.";
				}
				else
				{
					errorMessage = "Nieprawidłowy email lub hashasłoo.";
				}
			}
			catch (Exception ex)
			{
				errorMessage = $"Błąd połączenia: {ex.Message}";
			}
			finally
			{
				isLoading = false;
			}
		}
	}
}