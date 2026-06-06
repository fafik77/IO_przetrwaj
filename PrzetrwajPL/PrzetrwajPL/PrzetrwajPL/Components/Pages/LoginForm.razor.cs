using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Przetrwaj.CommonLibrary.Consts;
using PrzetrwajPL.Models;
using PrzetrwajPL.Requests;


namespace PrzetrwajPL.Components.Pages
{
	public partial class LoginForm
	{
		[CascadingParameter]
		private HttpContext? httpContext { get; set; }

		[SupplyParameterFromForm]
		public LoginRequest loginRequest { get; set; } = new LoginRequest();
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
				var response = await client.PostAsJsonAsync("/Login/email", loginRequest);
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
					if (httpContext?.Response != null)
					{
						httpContext.Response.Redirect(redirectUrl);
					}
					else
					{
						// Force a true page load so the AccountController can establish a standard HTTP Cookie context
						NavigationManager.NavigateTo(redirectUrl, forceLoad: true);
					}
				}
				else if (response.StatusCode == (System.Net.HttpStatusCode)StatusCodes.Status418ImATeapot)
				{
					var BanInfo = await response.Content.ReadFromJsonAsync<BanInfo>();
					errorMessage = $"Twoje konto zosta這 zablokowane przez {BanInfo.BannedBy.Name} {BanInfo.BannedBy.Surname}. Pow鏚: {BanInfo.BanReason}";
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
				{
					var errorText = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
					errorMessage = "Nieprawid這wy email lub has這.";
				}
				else
				{
					errorMessage = "Nieprawid這wy email lub has這.";
				}
			}
			catch (Exception ex)
			{
				errorMessage = $"B章d po章czenia: {ex.Message}";
			}
			finally
			{
				isLoading = false;
			}
		}
	}
}