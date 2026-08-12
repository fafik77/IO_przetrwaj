using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Przetrwaj.CommonLibrary.Consts;

namespace PrzetrwajPL.Components.Pages.Auth.Logout;

public partial class Logout
{
	[Inject]
	private IHttpClientFactory ClientFactory { get; set; } = default!;

	[CascadingParameter]
	private HttpContext? HttpContext { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		if (HttpContext?.User?.Identity?.IsAuthenticated == true)
		{
			try
			{
				// Attach the Auth Header token
				var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);

				// invalidate the refresh token on the API side
				await client.PostAsync("Account/logout", null);
			}
			catch (Exception ex)
			{
				// Log the exception but do not block the execution flow. 
				// We still want to clear local cookies even if the backend is down.
				Console.WriteLine($"B��d podczas uniewa�niania tokenu w API: {ex.Message}");
			}

			// Clear local application session cookies
			await HttpContext.SignOutAsync();

			// Redirect to the root page
			HttpContext.Response.Redirect("/");
		}
	}
}