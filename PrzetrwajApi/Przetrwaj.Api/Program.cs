using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Przetrwaj.Application;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture;
using Przetrwaj.Infrastucture.Context;
using Przetrwaj.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Bind the "Email" section to the EmailSettings class
builder.Services.Configure<EmailSettings>(
	builder.Configuration.GetSection("Email"));
// Bind the "Attachments" section to the AttachmentSettings class
builder.Services.Configure<AttachmentSettings>(
	builder.Configuration.GetSection("Attachments"));
// Bind the "FrontEnd" section to the FrontEndSettings class
builder.Services.Configure<FrontEndSettings>(
	builder.Configuration.GetSection("FrontEnd"));
// Bind the "OAuth" section to the OAuth class
builder.Services.Configure<OAuth>(
	builder.Configuration.GetSection("OAuth"));
// Bind the "Jwt" section to the JwtSettings class
builder.Services.Configure<JwtSettings>(
	builder.Configuration.GetSection("Jwt"));
// the bound sections
var oauthSettings = builder.Configuration.GetSection("OAuth").Get<OAuth>();
var frontEndSettings = builder.Configuration.GetSection("FrontEnd").Get<FrontEndSettings>();
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

// 1. Add the handler to the container
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// 2. Add the problem details service (standardizes error responses)
builder.Services.AddProblemDetails();

#region CORS Access-Control-Allow-Origin
var AllowAllOrigins = "_AllowAllOrigins";
var AllowPrzetrwajOrigins = "_AllowPrzetrwajOrigins";
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: AllowPrzetrwajOrigins,
		policy =>
		{
			policy.
			WithOrigins(
				"https://localhost:7173",
				"https://localhost",
				frontEndSettings.Url
			)
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
		});
	options.AddPolicy(name: AllowAllOrigins,
		policy =>
		{
			policy.
			AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod();
		});
});
#endregion

#region Auth
if (jwtSettings is null || jwtSettings.KeyBytes.Length <= 20) throw new ArgumentException("Invalid JWT.Key (Not set)");
builder.Services.AddAuthentication(
options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.IncludeErrorDetails = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.KeyBytes),
		ValidIssuer = jwtSettings.Issuer,
		ValidAudience = jwtSettings.Audience,
		ClockSkew = TimeSpan.FromSeconds(5),
		ValidateIssuerSigningKey = true,
		ValidateIssuer = true,
		ValidateLifetime = true,
		ValidateAudience = true,
		//ValidAlgorithms = [SecurityAlgorithms.HmacSha256Signature],
	};
})
.AddGoogle(options =>
{
	options.ClientId = oauthSettings?.Google?.ClientId ?? string.Empty;
	options.ClientSecret = oauthSettings?.Google?.ClientSecret ?? string.Empty;
	// This maps the Google claim to the standard .NET NameIdentifier
	options.SignInScheme = IdentityConstants.ExternalScheme;
});
// cookie for multiple .Net apps https://learn.microsoft.com/en-us/aspnet/core/security/cookie-sharing?view=aspnetcore-9.0
builder.Services.AddAuthorization(opt =>
{
	// Policy 1: User+ access (can add posts ...)
	opt.AddPolicy(UserRoles.User, policy =>
	{   // this is an or gate
		policy.RequireAuthenticatedUser(); //any registered user with any role that is able to log in
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

builder.Services.AddMemoryCache(); // for caching banned users (data is per user not per app)
builder.Services.AddLazyCache(); // for caching Statistics without Cache Stampede
builder.Services.AddInfrastructure(builder.Configuration);

// Add Identity services (This is the crucial step)
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
.AddEntityFrameworkStores<ApplicationDbContext>() // Specifies that Identity should use EF Core and this DbContext
.AddDefaultTokenProviders(); // Required for generating tokens (e.g., password reset)

builder.Services.AddAuthentication(options =>   //re-apply JWT as default
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IUrlHelper>(x =>
{
	var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
	var factory = x.GetRequiredService<IUrlHelperFactory>();
	return factory.GetUrlHelper(actionContext!);
});
// have this for accessing HttpContext anywhere
builder.Services.AddHttpContextAccessor();


builder.Services.AddApplication();
builder.Services.AddPresentation();



var app = builder.Build();

// MUST use the middleware early in the pipeline
app.UseExceptionHandler();

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

app.UseRouting(); // Added this explicitly beffore UseCors (New .Net thing)
app.UseCors(AllowPrzetrwajOrigins);
app.UsePresentation();



app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.Run();
