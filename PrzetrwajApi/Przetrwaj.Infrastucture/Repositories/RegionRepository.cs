using LazyCache;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Helpers;
using Przetrwaj.Domain.Models;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;


public class RegionRepository : IRegionRepository
{
	private readonly IDbContextFactory<ApplicationDbContext> _contextFactoryRO;
	private readonly ApplicationDbContext _dbContext;
	private readonly IAppCache _cache;
	private const string RegionsCacheKey = "Regions";
	private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24); // Long duration for static data

	public RegionRepository(IAppCache cache, IDbContextFactory<ApplicationDbContext> contextFactory, ApplicationDbContext dbContext)
	{
		_cache = cache;
		_contextFactoryRO = contextFactory;
		_dbContext = dbContext;
	}

	public async Task<AllRegions> GetAllAsync(CancellationToken ct)
	{
		// Fetch all regions into memory atomically (asynchronus querries)
		return await _cache.GetOrAddAsync(RegionsCacheKey, async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = _cacheDuration;

			// IMPORTANT: Use AsNoTracking because these objects will live in RAM
			var wojTask = ExecuteQueryAsync(ctx =>
				ctx.RegionWoj.AsNoTracking().OrderBy(r => r.Id).ToListAsync(ct));
			var powTask = ExecuteQueryAsync(ctx =>
				ctx.RegionPow.AsNoTracking().OrderBy(r => r.Id).ToListAsync(ct));
			var gmiTask = ExecuteQueryAsync(ctx =>
				ctx.RegionGmi.AsNoTracking().OrderBy(r => r.Id).ToListAsync(ct));

			await Task.WhenAll(wojTask, powTask, gmiTask);

			var regions = new AllRegions
			{
				Woj = await wojTask,
				Pow = await powTask,
				Gmi = await gmiTask
			};

			var list = new List<IRegionInfo>(regions.Woj);
			list.AddRange(regions.Pow);
			list.AddRange(regions.Gmi);
			regions.CompundList = list.OrderBy(r => r.Id).ToList();
			return regions;
		});
	}
	private async Task<T> ExecuteQueryAsync<T>(Func<ApplicationDbContext, Task<T>> query)
	{
		using var context = await _contextFactoryRO.CreateDbContextAsync();
		return await query(context);
	}

	public async Task<IRegionInfo?> GetByIdAsync(int id, CancellationToken ct)
	{
		var regionId = RegionCompoundHelper.UnifyRegionId(id);
		// Don't go to DB. Use the cached list.
		var allRegions = await GetAllAsync(ct);
		return allRegions.CompundList.FirstOrDefault(r => r.Id == regionId);
	}

	public async Task AddAsync<T>(IEnumerable<T> regions, CancellationToken ct) where T : class, IRegionInfo
	{
		await _dbContext.Set<T>().AddRangeAsync(regions, ct);
		_cache.Remove(RegionsCacheKey); //invalidate cache
	}

	public async Task AddAsync<T>(T region, CancellationToken ct) where T : class, IRegionInfo
	{
		await _dbContext.Set<T>().AddAsync(region, ct);
		_cache.Remove(RegionsCacheKey); //invalidate cache
	}

	public void Delete<T>(IEnumerable<T> regions) where T : class, IRegionInfo
	{
		_dbContext.Set<T>().RemoveRange(regions);
		_cache.Remove(RegionsCacheKey); //invalidate cache
	}

	public void Delete<T>(T region) where T : class, IRegionInfo
	{
		_dbContext.Set<T>().Remove(region);
		_cache.Remove(RegionsCacheKey); //invalidate cache
	}

	public void Update<T>(IEnumerable<T> regions) where T : class, IRegionInfo
	{
		_dbContext.Set<T>().UpdateRange(regions);
		_cache.Remove(RegionsCacheKey); //invalidate cache
	}

	public void Update<T>(T region) where T : class, IRegionInfo
	{
		_dbContext.Set<T>().Update(region);
		_cache.Remove(RegionsCacheKey); //invalidate cache
	}
}