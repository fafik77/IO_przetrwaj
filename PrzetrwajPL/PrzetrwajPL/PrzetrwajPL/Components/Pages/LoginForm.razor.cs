using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using PrzetrwajPL.Models;
using System.Security.Claims;

namespace PrzetrwajPL.Components.Pages
{
	public partial class LoginForm
	{
		[CascadingParameter]
		private HttpContext? httpContext { get; set; }
		//[CascadingParameter]
		//private NavigationManager NavigationManager { get; set; }

		private User? user = null;
		[SupplyParameterFromForm]
		public LoginRequest loginRequest { get; set; } = new LoginRequest();
		private string errorMessage = string.Empty;
		private bool isLoading = false;

		private async Task HandleLogin()
		{
			isLoading = true;
			errorMessage = string.Empty;
			try
			{
				var response = await HttpClient.PostAsJsonAsync("/Login/email", loginRequest);
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<User>();
					if (result != null)
					{
						user = result;
						var claims = new List<Claim>
							{
								new Claim(ClaimTypes.NameIdentifier, user.Id),
								new Claim(ClaimTypes.Name, user.Name ?? user.Email!), // Use Name for display
								new Claim(ClaimTypes.Email, user.Email!),
								new Claim(ClaimTypes.Role, user.Role ?? "User")
								// Add other properties like Region, Surname, etc., as custom claims if needed
							};

						var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						var userPrincipal = new ClaimsPrincipal(identity);
						await httpContext.SignInAsync(userPrincipal); //make cookie
						httpContext.Response.Redirect("/"); //use this method to redirect user, as the NavigateTo does throw an exception
					}
					else
					{
						errorMessage = "Nieprawid³owa odpowiedŸ z serwera.";
					}
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
				{
					User? result = null;
					try
					{
						result = await response.Content.ReadFromJsonAsync<User>();
						if (result?.Banned == true)
							errorMessage = $"Twoje konto zosta³o zablokowane przez {result.BannedBy.Name} {result.BannedBy.Surname}. Powód: {result.BanReason}";
					}
					catch (Exception ex)
					{
						errorMessage = "Nieprawid³owy email lub has³o.";
					}
				}
				else
				{
					var errorText = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
					errorMessage = "Nieprawid³owy email lub has³o.";
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