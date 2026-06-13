using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Przetrwaj.Application.ValidationPipeline;
using System.Text.Json.Serialization;

namespace Przetrwaj.Presentation;

public static class Extensions
{
	public static IServiceCollection AddPresentation(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();
		//services.AddOpenApi();
		services.AddSwaggerGen(options =>
		{
			options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
			{
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = JwtBearerDefaults.AuthenticationScheme
			});
			options.EnableAnnotations();
			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = JwtBearerDefaults.AuthenticationScheme
						}
					},
					Array.Empty<string>()
				}
			});
		});

		services.AddControllers()
		.AddJsonOptions(options =>
		{
			// This converts enums to strings globally
			options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});

		return services;
	}

	public static IApplicationBuilder UsePresentation(this WebApplication app)
	{
		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			//app.MapOpenApi();
			app.MapSwagger();
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		//the order matters
		app.UseAuthentication();
		app.UseMiddleware<UserBlackListMiddleware>();
		app.UseAuthorization();

		app.MapControllers();
		return app;
	}
}
