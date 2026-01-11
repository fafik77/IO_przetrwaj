using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using PrzetrwajPL.Models;
using PrzetrwajPL.Requests;

namespace PrzetrwajPL.Components.Pages
{
	public partial class RegisterForm
	{
		//private UserWithPersonalDataDto user = new();
		private RegisterRequest registerRequest = new();
		private string errorMessage = string.Empty;
		private bool isLoading = false;
		//private string selectedRegionDisplay = "Wybierz swój region";

		private async Task HandleRegister()
		{
			if (registerRequest.ConfirmPassword != registerRequest.Password)
			{
				errorMessage = "Has³a nie s¹ takie same!";
				return;
			}
			isLoading = true;
			errorMessage = string.Empty;
			try
			{
				var response = await HttpClient.PostAsJsonAsync("/Register/email", registerRequest);
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<UserWithPersonalDataDto>();
					if (result != null)
					{
						// Prepare the query parameters for the success page
						var queryParams = new Dictionary<string, string?>
						{
							["Name"] = registerRequest.Name,
							["Email"] = registerRequest.Email
						};
						// Build the URL: /registration-success?Name=Jan&Email=jan@example.com
						var successUrl = QueryHelpers.AddQueryString("/registration-success", queryParams);
						// Redirect the user
						NavigationManager.NavigateTo(successUrl);
					}
					else
					{
						var errorResult = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
						errorMessage = errorResult?.Error?.Message ?? "Wyst¹pi³ nieoczekiwany b³¹d.";
					}
				}
				else
				{
					var errorText = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
					// errorMessage = "Nieprawid³owy email lub has³o.";
					errorMessage = errorText?.Error.Message;
				}
			}
			catch (Exception ex)
			{
				errorMessage = $"B³¹d po³¹czenia: {ex.Message}";
			}
			finally
			{
				isLoading = false;
			}
		}
	}
}