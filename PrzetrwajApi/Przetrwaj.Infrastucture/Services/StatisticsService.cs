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
	private readonly ApplicationDbContext _context;
	private readonly IAppCache _cache;
	private const string StatsCacheKey = "Statistics";

	public StatisticsService(ApplicationDbContext context, IAppCache cache)
	{
		_context = context;
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
			var regionsTask = _context.Regions.LongCountAsync(cancellationToken);
			var usersTask = _context.Users.LongCountAsync(cancellationToken);
			var activeDangersTask = _context.Posts.LongCountAsync(p => p.Category == CategoryType.Danger && p.Active, cancellationToken);
			var activeResourcesTask = _context.Posts.LongCountAsync(p => p.Category == CategoryType.Resource && p.Active, cancellationToken);
			//_userManager.GetUsersInRoleAsync(UserRoles.Moderator)
			var moderatorCount = await _context.UserRoles
			.AsNoTracking()
			.Join(_context.Roles,
				ur => ur.RoleId,
				r => r.Id,
				(ur, r) => new { ur, r })
			.Where(joined => joined.r.Name == UserRoles.Moderator)
			.LongCountAsync(cancellationToken);

			await Task.WhenAll(regionsTask, usersTask, activeDangersTask, activeResourcesTask);
			if (cancellationToken.IsCancellationRequested)
				throw new TaskCanceledException();

			return new StatisticsDto
			{
				Regions = await regionsTask,
				Users = await usersTask,
				ActiveDangers = await activeDangersTask,
				ActiveResources = await activeResourcesTask,
				Moderators = moderatorCount
			};
		});
	}

	public async Task<StatisticsDto?> GetCachedStatisticsOnlyAsync(CancellationToken cancellationToken)
	{
		return await _cache.GetAsync<StatisticsDto?>(StatsCacheKey);
	}
}
