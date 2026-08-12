using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Requests;
using PrzetrwajPL.Components.Pages.Components;
using System.Security.Claims;
using System.Text;

namespace PrzetrwajPL.Components.Pages.Auth;

public partial class UserSettings
{
	// Access basic user info from the cookie claims
	[CascadingParameter]
	private Task<AuthenticationState> AuthStateTask { get; set; }
	[SupplyParameterFromQuery]
	public string? Success { get; set; }

	private UpdateAccountCommand UserUpdateRequest { get; set; } = new UpdateAccountCommand();
	private bool isLoading = false;
	private string errorMessage = string.Empty;
	private string successMessage = string.Empty;
	private RegionPicker? myRegionPicker;
	private bool isRegionPickerOpen = false;
	private ImpedimentsCheckboxGrid? preferencesCheckboxGrid;
	private bool isPreferencesCheckboxGridOpen = false;
	private int userRegionId = -1;
	private string? oldEmail;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await GetUserInfo();
			StateHasChanged();
		}
	}

	private void HandleRegionChanged(int id) => UserUpdateRequest.GminaId = id;
	private bool ShouldShowRegionPicker() => UserUpdateRequest.GminaId <= 0 || isRegionPickerOpen;
	private void ToggleRegionPicker() => isRegionPickerOpen = !isRegionPickerOpen;
	private void TogglePreferencesPicker() => isPreferencesCheckboxGridOpen = !isPreferencesCheckboxGridOpen;


	private async Task GetUserInfo()
	{
		isLoading = true;
		try
		{
			var authState = await AuthStateTask;
			var userPrincipal = authState.User;

			// start fetching full user data from API
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var getUserDetailsTask = client.GetAsync("/Account");

			// in the meantime populate something from the token
			try
			{
				var regionStr = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimNames.Region)?.Value;
				UserUpdateRequest.Email = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
				UserUpdateRequest.Name = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimNames.Name)?.Value;
				UserUpdateRequest.Surname = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimNames.Surname)?.Value;
				var impedimentsStr = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimNames.Impediments)?.Value;
				if (!string.IsNullOrEmpty(regionStr)) UserUpdateRequest.GminaId = userRegionId = int.Parse(regionStr);
				if (!string.IsNullOrEmpty(impedimentsStr)) UserUpdateRequest.Impediments = int.Parse(impedimentsStr);
			}
			catch (Exception)
			{
				// fail silently as this was only the local cookie which might have been corrupted or missing!
			}



			// populate everything from the user info (if not redirected from refresh)
			if (string.IsNullOrEmpty(Success))
				await GetUserDetailsFromApi(getUserDetailsTask);
			else
				//show the succesfully applied message when redirected from this:"/account/refresh-cookie?"...
				successMessage = Success;

			//otherwise only populate from local data
			oldEmail = UserUpdateRequest.Email;
			await myRegionPicker.LoadRegionFromId(userRegionId);
			await preferencesCheckboxGrid.LoadFromIntBitFieldAsync(UserUpdateRequest.Impediments ?? 0);
		}
		catch (Exception ex)
		{
			errorMessage = $"Błąd podczas pobierania danych: {ex.Message}";
		}
		finally
		{
			isLoading = false;
		}
	}

	private async Task GetUserDetailsFromApi(Task<HttpResponseMessage> getUserDetailsTask)
	{
		var userDetails = await getUserDetailsTask;
		if (userDetails.IsSuccessStatusCode)
		{
			var userInfo = await userDetails.Content.ReadFromJsonAsync<UserWithPersonalDataDto>();
			if (userInfo != null)
			{
				// Map data to the update command
				UserUpdateRequest.Name = userInfo.Name;
				UserUpdateRequest.Surname = userInfo.Surname;
				UserUpdateRequest.Email = userInfo.Email;
				UserUpdateRequest.Impediments = userInfo.Impediments;

				// Store region ID to set the picker later
				if (userInfo.Region != null)
					UserUpdateRequest.GminaId = userRegionId = userInfo.Region.Id;
			}
		}
		else
		{
			errorMessage = "Nie udało się pobrać danych użytkownika.";
		}
	}

	private async Task HandleInvalid()
	{
		errorMessage = "W formularzu znajdują się niepoprawne danę!";
		return;
	}

	private async Task HandleUpdate()
	{
		isLoading = true;
		errorMessage = string.Empty;
		successMessage = string.Empty;
		const string settingsPage = "settings";

		try
		{
			var selectedPreferences = preferencesCheckboxGrid?.GetSelectedAsIntBitField();
			UserUpdateRequest.Impediments = selectedPreferences;

			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			UserUpdateRequest.ReturnUrl = NavigationManager.BaseUri;
			var response = await client.PutAsJsonAsync("/Account", UserUpdateRequest);
			bool IsSuccessStatusCode = response.IsSuccessStatusCode;
			List<HttpContent> errorsContent = [];
			if (!response.IsSuccessStatusCode) errorsContent.Add(response.Content);

			//change password
			if (!string.IsNullOrWhiteSpace(UserUpdateRequest.NewPassword))
			{
				//we can send the entire object, it will be stripped by the API
				response = await client.PatchAsJsonAsync("/Account/password", UserUpdateRequest);
				//both requests have to sucseed
				IsSuccessStatusCode &= response.IsSuccessStatusCode;
				if (!response.IsSuccessStatusCode) errorsContent.Add(response.Content);
			}

			//new email
			if (!UserUpdateRequest.Email.Equals(oldEmail, comparisonType: StringComparison.OrdinalIgnoreCase))
			{
				//we can send the entire object, it will be stripped by the API
				response = await client.PatchAsJsonAsync("/Account/email", UserUpdateRequest);
				//both requests have to sucseed
				IsSuccessStatusCode &= response.IsSuccessStatusCode;
				if (!response.IsSuccessStatusCode) errorsContent.Add(response.Content);
			}


			if (IsSuccessStatusCode)
			{
				// Trigger the cookie update handshake (Force page reload to refresh cookie)
				// We redirect to a server-side endpoint to issue a new cookie with updated claims
				NavigationManager.NavigateTo($"/account/refresh-cookie?redirectTo={settingsPage}", forceLoad: true);
			}
			else
			{
				StringBuilder sb = new();
				foreach (var error in errorsContent)
				{
					var errorContent = await error.ReadFromJsonAsync<ExceptionCasting>();
					sb.AppendLine(errorContent?.Error?.Message);
				}
				errorMessage = sb.ToString();
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

	private async Task ConfirmAndLogoutAll()
	{
		// localized Yes/No = Tak/Nie
		bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Czy na pewno chcesz wylogować wszystkie aktywne sesje na urządzeniach?");

		if (isConfirmed)
		{
			NavigationManager.NavigateTo("/logout-all-sessions");
		}
	}
}