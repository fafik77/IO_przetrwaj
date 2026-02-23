using LazyCache;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Services;

public class StatisticsService : IStatisticsService
{
	private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
	private readonly IRegionRepository _regionRepository;
	private readonly ICategoryRepository _categoryRepository;
	private readonly IAppCache _cache;
	private const string StatsCacheKey = "Statistics";
	private static readonly TimeSpan _statisticsCacheDuration = TimeSpan.FromHours(1);

	public StatisticsService(IDbContextFactory<ApplicationDbContext> contextFactory, IRegionRepository regionRepository, ICategoryRepository categoryRepository, IAppCache cache)
	{
		_contextFactory = contextFactory;
		_regionRepository = regionRepository;
		_categoryRepository = categoryRepository;
		_cache = cache;
	}

	/// <summary>
	/// Gets or Fetches StatisticsDto that is fetched only once an hour from DB. After that its cached.
	/// This method also prevents "Cache Stampede" (only 1 DB hit even if multiple users want the Statistics).
	/// </summary>
	/// <returns>StatisticsDto</returns>
	/// <exception cref="TaskCanceledException"></exception>
	public async Task<StatisticsDto> GetCachedOrFetchStatisticsAsync(CancellationToken cancellationToken)
	{
		// Try to get from cache, or fetch and save if not present
		return await _cache.GetOrAddAsync(StatsCacheKey, async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = _statisticsCacheDuration;
			// Log this to see when a REAL DB hit happens
			Console.WriteLine("Cache expired. Fetching fresh statistics from Database...");

			// Run the parallel counts
			var usersTask = GetCountAsync(ctx => ctx.Users.LongCountAsync());
			var activeDangersTask = GetCountAsync(ctx => ctx.Posts.LongCountAsync(p => p.CategoryType == CategoryType.Danger && p.Active));
			var activeResourcesTask = GetCountAsync(ctx => ctx.Posts.LongCountAsync(p => p.CategoryType == CategoryType.Resource && p.Active));
			var moderatorsTask = GetCountAsync(ctx => ctx.UserRoles
			.AsNoTracking()
			.Join(ctx.Roles,
				ur => ur.RoleId,
				r => r.Id,
				(ur, r) => new { ur, r })
			.Where(joined => joined.r.Name == UserRoles.Moderator)
			.LongCountAsync());

			///here I am hoping that some tasks will complete beffore reaching the `regionsTask` which has 3 more async queries if not already in RAM.
			///_categoryRepository gets the entire list of Danger/Resource Category and stores them in RAM, running 2 async queries is bad.
			var DangerCategories = await _categoryRepository.GetDangersAsync(cancellationToken);
			var ResourceCategories = await _categoryRepository.GetResourcesAsync(cancellationToken);
			var regionsTask = _regionRepository.GetAllAsync(cancellationToken);

			//get all of their results
			await Task.WhenAll(regionsTask, usersTask, activeDangersTask, activeResourcesTask);
			//return those results
			return new StatisticsDto
			{
				RegionsWoj = (await regionsTask).Woj.Count,
				RegionsPow = (await regionsTask).Pow.Count,
				RegionsGmi = (await regionsTask).Gmi.Count,
				DangerCategories = DangerCategories.Count(),
				ResourceCategories = ResourceCategories.Count(),
				Users = await usersTask,
				ActiveDangers = await activeDangersTask,
				ActiveResources = await activeResourcesTask,
				Moderators = await moderatorsTask
			};
		});
	}

	public async Task<StatisticsDto?> GetCachedStatisticsOnlyAsync(CancellationToken cancellationToken)
	{
		return await _cache.GetAsync<StatisticsDto?>(StatsCacheKey);
	}


	/// <summary>
	/// ! Warning context amount is limited, use it sparingly !
	/// Helper method to manage the lifecycle of the temporary context. creates a new context for each task and runs it.
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	private async Task<long> GetCountAsync(Func<ApplicationDbContext, Task<long>> query)
	{
		using var context = await _contextFactory.CreateDbContextAsync();
		return await query(context);
	}
}
