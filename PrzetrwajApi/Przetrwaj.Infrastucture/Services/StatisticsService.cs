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
	private readonly IAppCache _cache;
	private const string StatsCacheKey = "Statistics";

	public StatisticsService(IDbContextFactory<ApplicationDbContext> contextFactory, IAppCache cache)
	{
		_contextFactory = contextFactory;
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
			// Set cache duration (e.g., 5 minutes)
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
			// Log this so you can see in your console when a REAL DB hit happens
			Console.WriteLine("Cache expired. Fetching fresh statistics from Database...");

			// Run the parallel counts
			var regionsTask = GetCountAsync(ctx => ctx.Regions.LongCountAsync());
			var usersTask = GetCountAsync(ctx => ctx.Users.LongCountAsync());
			var activeDangersTask = GetCountAsync(ctx => ctx.Posts.LongCountAsync(p => p.Category == CategoryType.Danger && p.Active));
			var activeResourcesTask = GetCountAsync(ctx => ctx.Posts.LongCountAsync(p => p.Category == CategoryType.Resource && p.Active));
			//_userManager.GetUsersInRoleAsync(UserRoles.Moderator)
			var moderatorsTask = GetCountAsync(ctx => ctx.UserRoles
			.AsNoTracking()
			.Join(ctx.Roles,
				ur => ur.RoleId,
				r => r.Id,
				(ur, r) => new { ur, r })
			.Where(joined => joined.r.Name == UserRoles.Moderator)
			.LongCountAsync());
			//get all of their results
			await Task.WhenAll(regionsTask, usersTask, activeDangersTask, activeResourcesTask);
			//return those results
			return new StatisticsDto
			{
				Regions = await regionsTask,
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
