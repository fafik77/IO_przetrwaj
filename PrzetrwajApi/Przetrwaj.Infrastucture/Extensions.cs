using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Infrastucture.Context;
using Przetrwaj.Infrastucture.Repositories;
using Przetrwaj.Infrastucture.Services;

namespace Przetrwaj.Infrastucture
{
	public static class Extensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("Database");
			services.AddDbContext<ApplicationDbContext>(ctx => ctx.UseNpgsql(connectionString));

			services.AddScoped<IUnitOfWork, UnitOfWork>();  //AddScoped makes this per request, Transient makes a new instance every time its called
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IRegionRepository, RegionRepository>();
			services.AddScoped<ICategoryRepository, CategoryRepository>();
			services.AddScoped<IPostRepository, PostRepository>();
			services.AddScoped<IAttachmentRepository, AttachmentRepository>();
			services.AddScoped<IStatisticsService, StatisticsService>();

			return services;
		}
	}
}

