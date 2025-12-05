using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Przetrwaj.Presentation;

public static class Extensions
{
	public static IServiceCollection AddPresentation(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();
		//services.AddOpenApi();
		services.AddSwaggerGen(swagger => swagger.EnableAnnotations());

		services.AddControllers();

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
		app.UseAuthorization();

		app.MapControllers();
		return app;
	}
}
