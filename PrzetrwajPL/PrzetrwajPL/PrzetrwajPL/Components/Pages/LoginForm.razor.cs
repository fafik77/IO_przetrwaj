using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using PrzetrwajPL.Models;
using PrzetrwajPL.Requests;
using System.Security.Claims;


namespace PrzetrwajPL.Components.Pages
{
	public partial class LoginForm
	{
		[CascadingParameter]
		private HttpContext? httpContext { get; set; }
		//[CascadingParameter]
		//private NavigationManager NavigationManager { get; set; }

		private UserWithPersonalDataDto? user = null;
		[SupplyParameterFromForm]
		public LoginRequest loginRequest { get; set; } = new LoginRequest();
		private string errorMessage = string.Empty;
		private bool isLoading = false;

		private string GoogleLoginUrl => $"{ClientFactory.CreateClient("ServerAPI").BaseAddress}Login/google";

		protected override async Task OnInitializedAsync()
		{
			var authState = await AuthStateProvider.GetAuthenticationStateAsync();
			if (authState.User.Identity.IsAuthenticated)
			{
				// The cookie is now in the browser, so this call will work!
				var client = ClientFactory.CreateClient("ServerAPI");
				var user = await client.GetFromJsonAsync<UserWithPersonalDataDto>("Account/");
				await LoginUser(user);
			}
			await base.OnInitializedAsync();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				var authState = await AuthStateProvider.GetAuthenticationStateAsync();
				if (authState.User.Identity.IsAuthenticated)
				{
					// The cookie is now in the browser, so this call will work!
					var client = ClientFactory.CreateClient("ServerAPI");
					var user = await client.GetFromJsonAsync<UserWithPersonalDataDto>("Account/");
					await LoginUser(user);
				}
			}
			await base.OnAfterRenderAsync(firstRender);
		}

		private async Task HandleLogin()
		{
			isLoading = true;
			errorMessage = string.Empty;
			try
			{
				var client = ClientFactory.CreateClient("ServerAPI");
				var response = await client.PostAsJsonAsync("/Login/email", loginRequest);
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<UserWithPersonalDataDto>();
					await LoginUser(result);
				}
				else if (response.StatusCode == (System.Net.HttpStatusCode)StatusCodes.Status418ImATeapot)
				{
					var bannedUser = await response.Content.ReadFromJsonAsync<UserWithPersonalDataDto>();
					errorMessage = $"Twoje konto zosta³o zablokowane przez {bannedUser.BannedBy.Name} {bannedUser.BannedBy.Surname}. Powód: {bannedUser.BanReason}";
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
				{
					errorMessage = "Nieprawid³owy email lub has³o.";
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

		private async Task LoginUser(UserWithPersonalDataDto? userToLogin)
		{
			if (userToLogin == null)
			{
				errorMessage = "Nieprawid³owa odpowiedŸ z serwera.";
				return;
			}
			user = userToLogin;
			var claims = new List<Claim>
							{
								new Claim(ClaimTypes.NameIdentifier, user.Id),
								new Claim(ClaimTypes.Name, user.Name ?? user.Email!), // Use Name for display
								new Claim(ClaimTypes.Email, user.Email!),
							};
			// Split the roles string (e.g., "User,Moderator") and add each as a separate claim
			if (!string.IsNullOrWhiteSpace(user.Role))
			{
				var roles = user.Role.Split(',', StringSplitOptions.RemoveEmptyEntries);
				foreach (var role in roles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role.Trim())); // remember to trim
				}
			}
			else
			{
				claims.Add(new Claim(ClaimTypes.Role, UserRoles.User));
			}

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var userPrincipal = new ClaimsPrincipal(identity);
			await httpContext.SignInAsync(userPrincipal); //make cookie
			httpContext.Response.Redirect("/"); //use this method to redirect user, as the NavigateTo does throw an exception
		}
	}
}