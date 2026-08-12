using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Requests;

namespace PrzetrwajPL.Components.Pages.Auth.Register;

public partial class RegisterForm
{
	private readonly RegisterRequest registerRequest = new();
	private string errorMessage = string.Empty;
	private bool isLoading = false;

	private async Task HandleRegister()
	{
		if (registerRequest.ConfirmPassword != registerRequest.Password)
		{
			errorMessage = "Hasła nie są takie same!";
			return;
		}
		isLoading = true;
		errorMessage = string.Empty;
		try
		{
			registerRequest.ReturnUrl = NavigationManager.BaseUri;
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var response = await client.PostAsJsonAsync("/Register/email", registerRequest);
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
					// Build the URL:
					var successUrl = QueryHelpers.AddQueryString("/registration-success", queryParams);
					// Redirect the user
					NavigationManager.NavigateTo(successUrl);
				}
				else
				{
					var errorResult = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
					errorMessage = errorResult?.Error?.Message ?? "Wystąpił nieoczekiwany błąd.";
				}
			}
			else
			{
				var errorText = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
				errorMessage = errorText?.Error.Message ?? "";
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