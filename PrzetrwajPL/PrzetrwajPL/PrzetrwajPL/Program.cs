using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Przetrwaj.CommonLibrary.Consts;
using PrzetrwajPL.Components;
using PrzetrwajPL.Handlers;
using PrzetrwajPL.Handlers.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

#region Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.Name = Consts.PrzetrwajAuthCookie;
		options.LoginPath = "/login";

		options.Events = new CookieAuthenticationEvents
		{
			OnValidatePrincipal = RefreshJwtTokenOnValidatePrincipal.OnValidatePrincipal
		};
	});
builder.Services.AddAuthorization(opt =>
{
	// User+ : Requires any role
	opt.AddPolicy(UserRoles.User, policy =>
		policy.RequireAuthenticatedUser()); //any registered user with any role that is able to log in

	// Moderator+ : Requires either Moderator or Admin
	opt.AddPolicy(UserRoles.Moderator, policy =>
		policy.RequireRole(UserRoles.Moderator, UserRoles.Admin));

	// Admin only
	opt.AddPolicy(UserRoles.Admin, policy =>
		policy.RequireRole(UserRoles.Admin));
});
builder.Services.AddCascadingAuthenticationState();
#endregion //Auth

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtAuthorizationHandler>();
builder.Services.AddTransient<TokenRefreshHandler>();

//those settings are to this client alone
builder.Services.AddHttpClient(Consts.PrzetrwajApiClientName, client =>
{
	client.BaseAddress =
	//new Uri("https://przetrwaj-api.grayflower-7f624026.polandcentral.azurecontainerapps.io/");
	new Uri("https://localhost:7072/");
})
.AddHttpMessageHandler<JwtAuthorizationHandler>()   // include JWT in AuthHeader
.AddHttpMessageHandler<TokenRefreshHandler>();      // retry login with RefreshToken


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

app.MapControllers();

app.Run();
