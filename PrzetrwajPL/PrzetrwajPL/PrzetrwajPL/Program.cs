using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PrzetrwajPL;
using PrzetrwajPL.Components;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();
builder.Services.AddHttpClient("ServerAPI", client => 
{
	client.BaseAddress =
	new Uri("https://przetrwaj-api.grayflower-7f624026.polandcentral.azurecontainerapps.io/");
	//new Uri("https://localhost:7072/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
	// This allows the client to send and receive cookies automatically
	UseCookies = true,
	// Warning: On Blazor Server, sharing one CookieContainer in a Singleton 
	// can lead to users seeing each other's sessions. 
	// For InteractiveServer, the browser handles cookies naturally.
	//CookieContainer = new System.Net.CookieContainer(),
	//Credentials = System.Net.CredentialCache.DefaultCredentials
});

#region Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.Name = "cookie";
		options.Cookie.MaxAge = TimeSpan.FromHours(6);
		options.LoginPath = "/login";
	});
builder.Services.AddAuthorization(opt =>
{
	// User+ : Requires either User, Moderator, or Admin
	opt.AddPolicy(UserRoles.User, policy =>
		policy.RequireRole(UserRoles.User, UserRoles.Moderator, UserRoles.Admin));

	// Moderator+ : Requires either Moderator or Admin
	opt.AddPolicy(UserRoles.Moderator, policy =>
		policy.RequireRole(UserRoles.Moderator, UserRoles.Admin));

	// Admin only
	opt.AddPolicy(UserRoles.Admin, policy =>
		policy.RequireRole(UserRoles.Admin));
});
builder.Services.AddCascadingAuthenticationState();
#endregion //Auth

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(PrzetrwajPL.Client._Imports).Assembly);

app.Run();
