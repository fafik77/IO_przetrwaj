using Microsoft.AspNetCore.Components;
using PrzetrwajPL.Models;
using PrzetrwajPL.Requests;

namespace PrzetrwajPL.Components.Pages
{
	public partial class RegisterForm
	{
		private UserWithPersonalDataDto user = new();
		private RegisterRequest registerRequest = new();
		private string errorMessage = string.Empty;
		private bool isLoading = false;

		private async Task HandleRegister()
		{
			if (registerRequest.ConfirmPassword != registerRequest.Password)
			{
				errorMessage = "Has≥a nie sπ takie same!";
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
						//Po rejestracji wymagane jest potwierdzenie emaila a potem zalogowanie -PN
						NavigationManager.NavigateTo("/", forceLoad: true);
					}
					else
					{
						errorMessage = "Nieprawid≥owa odpowiedü z serwera.";
					}
				}
				else
				{
					var errorText = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
					// errorMessage = "Nieprawid≥owy email lub has≥o.";
					errorMessage = errorText?.Error.Message;
				}
			}
			catch (Exception ex)
			{
				errorMessage = $"B≥πd po≥πczenia: {ex.Message}";
			}
			finally
			{
				isLoading = false;
			}
		}
	}
}