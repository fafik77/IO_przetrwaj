using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Przetrwaj.Application.ValidationPipeline;
using System.Reflection;

namespace Przetrwaj.Application;

public static class Extensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var ExecutingAssembly = Assembly.GetExecutingAssembly();
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ExecutingAssembly));
		//services.AddAutoMapper(ExecutingAssembly);  ///Important breaking changes: since 15.0+ registration and purchase is required so you have to use 14.0  https://docs.automapper.io/en/stable/15.0-Upgrade-Guide.html

		services.AddValidatorsFromAssembly(ExecutingAssembly);  // 2. Register all Validators from the Application assembly
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // 3. Register the Validation Behavior as a MediatR Pipeline

		//services.AddScoped<IValidator<AddFilmCommand>, AddFilmCommandValidation>();
		//services.AddScoped<IValidator<UpdateFilmCommand>, UpdateFilmCommandValidation>();

		return services;
	}
}
