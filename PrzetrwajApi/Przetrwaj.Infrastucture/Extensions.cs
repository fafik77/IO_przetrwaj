using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Infrastucture.Context;
using Przetrwaj.Infrastucture.Repositories;

namespace Przetrwaj.Infrastucture
{
	public static class Extensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IUserRepository, UserRepository>();
			var conStr = configuration.GetConnectionString("Database");
			services.AddDbContext<ApplicationDbContext>(ctx => ctx.UseNpgsql(conStr));

			return services;
		}
	}
}

