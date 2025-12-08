using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Przetrwaj.Api;
using Przetrwaj.Application;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture;
using Przetrwaj.Infrastucture.Context;
using Przetrwaj.Presentation;

var builder = WebApplication.CreateBuilder(args);

const string AuthenticationCookie = "cookie";

builder.Services.Configure<EmailSettings>(
	builder.Configuration.GetSection("Email"));



#region CORS Access-Control-Allow-Origin
var AllowAllOrigins = "_AllowAllOrigins";
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: AllowAllOrigins,
					  policy =>
					  {
						  policy.AllowAnyOrigin()
						  .AllowAnyHeader()
						  .AllowAnyMethod();
					  });
});
#endregion

#region Auth
builder.Services.AddAuthentication(AuthenticationCookie)
	.AddCookie(AuthenticationCookie);
//.AddIdentityCookies();
// cookie for multiple .Net apps https://learn.microsoft.com/en-us/aspnet/core/security/cookie-sharing?view=aspnetcore-9.0
builder.Services.AddAuthorization(opt =>
{
	// Policy 1: User+ access (can add posts ...)
	opt.AddPolicy(UserRoles.User, policy =>
	{   // this is an or gate
		policy.RequireRole(UserRoles.User, UserRoles.Moderator, UserRoles.Admin);
	});

	// Policy 2: Moderator+ access
	opt.AddPolicy(UserRoles.Moderator, policy =>
	{
		// Only Moderators and Administrators can ...
		policy.RequireRole(UserRoles.Moderator, UserRoles.Admin);
	});

	// Policy 3: Administrator access (can manage moderators)
	opt.AddPolicy(UserRoles.Admin, policy =>
	{
		// Only Administrators can ...
		policy.RequireRole(UserRoles.Admin);
	});
});
#endregion

builder.Services.AddInfrastructure(builder.Configuration);

// 2. Add Identity services (This is the crucial step)
// It registers UserManager<AppUser>, SignInManager<AppUser>, and other core Identity services.
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
	// Password settings. 
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = true;
	options.Password.RequiredLength = 8;
	// Lockout settings.
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 7;
	options.Lockout.AllowedForNewUsers = true;
	//Other settings:
	options.User.RequireUniqueEmail = true;
	options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Specifies that Identity should use EF Core and your DbContext
.AddDefaultTokenProviders(); // Required for generating tokens (e.g., password reset)
builder.Services.ConfigureApplicationCookie(options =>
{
	// Cookie settings
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromHours(1);

	//options.LoginPath = "/Identity/Account/Login";
	//options.AccessDeniedPath = "/Identity/Account/AccessDenied";
	options.SlidingExpiration = true;
});

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IUrlHelper>(x =>
{
	var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
	var factory = x.GetRequiredService<IUrlHelperFactory>();
	return factory.GetUrlHelper(actionContext!);
});
// You almost certainly already have this for accessing HttpContext anywhere:
builder.Services.AddHttpContextAccessor();


builder.Services.AddApplication();
builder.Services.AddPresentation();
builder.Services.AddTransient<IEmailSender, EmailAzureService>();



var app = builder.Build();


app.UsePresentation();

//app.MapPost("login/email", () => "login email");
//app.MapPost("login/google", () => "login google");


app.Run();
