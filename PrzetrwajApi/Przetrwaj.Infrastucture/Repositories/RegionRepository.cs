using LazyCache;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;


public class RegionRepository : IRegionRepository
{
	private readonly ApplicationDbContext _dbContext;
	private readonly IAppCache _cache;
	private const string RegionsCacheKey = "Regions";
	private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24); // Long duration for static data

	public RegionRepository(ApplicationDbContext dbContext, IAppCache cache)
	{
		_dbContext = dbContext;
		_cache = cache;
	}

	public async Task<IEnumerable<Region>> GetAllAsync(CancellationToken ct)
	{
		// Fetch all regions into memory atomically
		return await _cache.GetOrAddAsync(RegionsCacheKey, async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
			// IMPORTANT: Use AsNoTracking because these objects will live in RAM
			return await _dbContext.Regions.AsNoTracking().ToListAsync(ct);
		});
	}

	public async Task<Region?> GetByIdAsync(int id, CancellationToken ct)
	{
		// Don't go to DB. Use the cached list.
		var allRegions = await GetAllAsync(ct);
		return allRegions.FirstOrDefault(r => r.IdRegion == id);
	}

	public async Task AddAsync(Region region, CancellationToken ct)
	{
		await _dbContext.Regions.AddAsync(region, ct);
		// We don't save changes here (Unit of Work pattern), but we MUST clear cache
		_cache.Remove(RegionsCacheKey);
	}

	public void Update(Region item)
	{
		_dbContext.Regions.Update(item);
		_cache.Remove(RegionsCacheKey);
	}

	public void Delete(Region item)
	{
		_dbContext.Regions.Remove(item);
		_cache.Remove(RegionsCacheKey);
	}
}