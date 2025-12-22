using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;

namespace PrzetrwajPL.Components.Pages
{
	public partial class Logout
	{
		[CascadingParameter]
		private HttpContext? httpContext { get; set; }
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			if (httpContext.User.Identity.IsAuthenticated)
			{
				//wyœlijcie request do `/account/logout` ¿eby wylogowaæ u¿ytkownika z api -PN
				await httpContext.SignOutAsync();
				httpContext.Response.Redirect("/logout"); //again use this over naviMgr
			}
		}
	}
}