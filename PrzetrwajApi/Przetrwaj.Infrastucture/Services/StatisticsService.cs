using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Services;

public class StatisticsService
{
	private readonly ApplicationDbContext _context;
	private readonly IMemoryCache _cache;
	private readonly UserManager<AppUser> _userManager;
	private const string StatsCacheKey = "Statistics";

	public StatisticsService(ApplicationDbContext context, IMemoryCache cache, UserManager<AppUser> userManager)
	{
		_context = context;
		_cache = cache;
		_userManager = userManager;
	}

	public async Task<StatisticsDto> GetCachedStatisticsAsync(CancellationToken cancellationToken)
	{
		// Try to get from cache, or fetch and save if not present
		return await _cache.GetOrCreateAsync(StatsCacheKey, async entry =>
		{
			// Set cache duration (e.g., 5 minutes)
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

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
			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

			StatisticsDto stats = new StatisticsDto
			{
				Regions = await regionsTask,
				Users = await usersTask,
				ActiveDangers = await activeDangersTask,
				ActiveResources = await activeResourcesTask,
				Moderators = moderatorCount
			};
			return stats;
		});
	}
}
