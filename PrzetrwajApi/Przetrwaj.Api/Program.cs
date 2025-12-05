using Przetrwaj.Application;
using Przetrwaj.Infrastucture;
using Przetrwaj.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//builder.Services.AddSwaggerGen();

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
builder.Services.AddAuthentication("cookie")
	.AddCookie("cookie");
#endregion

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddPresentation();



//builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//	app.MapOpenApi();
//	app.MapSwagger();
//	app.UseSwagger();
//	app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//the order matters
//app.UseAuthentication();
//app.UseAuthorization();

app.UsePresentation();

//app.MapPost("login/email", () => "login email");
//app.MapPost("login/google", () => "login google");

//app.MapPost("register/email", () => "register email");

//app.MapControllers();

app.Run();
