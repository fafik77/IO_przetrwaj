using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.FileProviders;
using Przetrwaj.Application;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture;
using Przetrwaj.Infrastucture.Context;
using Przetrwaj.Presentation;

var builder = WebApplication.CreateBuilder(args);

const string AuthenticationCookie = "cookie";

// Bind the "Email" section to the EmailSettings class
builder.Services.Configure<EmailSettings>(
	builder.Configuration.GetSection("Email"));
// Bind the "Attachments" section to the AttachmentSettings class
builder.Services.Configure<AttachmentSettings>(
	builder.Configuration.GetSection("Attachments")
);
// Bind the "FrontEnd" section to the FrontEndSettings class
builder.Services.Configure<FrontEndSettings>(
	builder.Configuration.GetSection("FrontEnd")
);

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

//builder.Services.AddMemoryCache(); // for caching Statistics
builder.Services.AddLazyCache(); // for caching Statistics without Cache Stampede
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
	options.ExpireTimeSpan = TimeSpan.FromHours(8);

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



var app = builder.Build();

#region Attachments
// Define the physical folder (outside the project root for safety)
string attachmentsPath = Path.Combine(builder.Environment.ContentRootPath, "Attachments");
// Ensure the directory exists
if (!Directory.Exists(attachmentsPath))
{
	Console.WriteLine($"Warning! Creating Attachments directory: {attachmentsPath}");
	Directory.CreateDirectory(attachmentsPath);
}
app.UseStaticFiles(new StaticFileOptions     //Allow serving <Image> in requests
{
	FileProvider = new PhysicalFileProvider(attachmentsPath),
	RequestPath = "/Attachments" // The URL prefix
});
#endregion //Attachments

app.UseCors(AllowAllOrigins);
app.UsePresentation();

//app.MapPost("login/email", () => "login email");
//app.MapPost("login/google", () => "login google");
//app.MapGet("/Attachments/", () => provides the given Attachment file (for frontend rendering as an image) );


app.Run();
