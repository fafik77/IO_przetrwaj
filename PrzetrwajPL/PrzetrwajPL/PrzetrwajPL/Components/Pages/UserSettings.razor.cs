using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Requests;
using PrzetrwajPL.Components.Pages.Components;
using System.Security.Claims;

namespace PrzetrwajPL.Components.Pages
{
	public partial class UserSettings
	{
		// Access basic user info from the cookie claims
		[CascadingParameter]
		private Task<AuthenticationState> AuthStateTask { get; set; }
		[SupplyParameterFromQuery]
		public string? Success { get; set; }

		private UpdateAccountCommand userUpdateRequest { get; set; } = new UpdateAccountCommand();
		private bool isLoading = false;
		private string errorMessage = string.Empty;
		private string successMessage = string.Empty;
		private RegionPicker myRegionPicker;
		private int userRegionId = -1;

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				await GetUserInfo();
				StateHasChanged();
			}
		}

		private async Task GetUserInfo()
		{
			isLoading = true;
			try
			{
				var authState = await AuthStateTask;
				var userPrincipal = authState.User;

				if (userPrincipal.Identity is not { IsAuthenticated: true })
				{
					NavigationManager.NavigateTo("/login");
					return;
				}

				try
				{
					var region = userPrincipal.Claims.FirstOrDefault(c => c.Type == "Region")?.Value;
					userUpdateRequest.Email = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
					userUpdateRequest.Name = userPrincipal.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
					userUpdateRequest.Surname = userPrincipal.Claims.FirstOrDefault(c => c.Type == "Surname")?.Value;
					if (!string.IsNullOrEmpty(region)) userUpdateRequest.GminaId = userRegionId = int.Parse(region);
				}
				catch (Exception) { }
				if (!string.IsNullOrEmpty(Success))
				{
					successMessage = Success;
					await myRegionPicker.LoadRegionFromId(userRegionId);
					return;
				}
				// 1. Fetch full user data from API
				var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
				var responseTask = client.GetAsync("/Account");

				var response = await responseTask;
				if (response.IsSuccessStatusCode)
				{
					var userInfo = await response.Content.ReadFromJsonAsync<UserWithPersonalDataDto>();
					if (userInfo != null)
					{
						// 2. Map data to the update command
						userUpdateRequest.Name = userInfo.Name;
						userUpdateRequest.Surname = userInfo.Surname;
						userUpdateRequest.Email = userInfo.Email;

						// Store region ID to set the picker later
						if (userInfo.Region != null)
						{
							userUpdateRequest.GminaId = userRegionId = userInfo.Region.Id;
						}
					}
				}
				else
				{
					errorMessage = "Nie uda³o siê pobraæ danych u¿ytkownika.";
				}
				await myRegionPicker.LoadRegionFromId(userRegionId);
			}
			catch (Exception ex)
			{
				errorMessage = $"B³¹d podczas pobierania danych: {ex.Message}";
			}
			finally
			{
				isLoading = false;
			}
		}

		private async Task HandleUpdate()
		{
			isLoading = true;
			errorMessage = string.Empty;
			successMessage = string.Empty;

			try
			{
				var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
				var response = await client.PutAsJsonAsync("/Account", userUpdateRequest);
				if (response.IsSuccessStatusCode)
				{
					// Trigger the cookie update handshake (Force page reload to refresh cookie)
					// We redirect to a server-side endpoint to issue a new cookie with updated claims
					NavigationManager.NavigateTo($"/account/refresh-cookie?redirectTo=settings", forceLoad: true);
				}
				else
				{
					var errorText = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
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