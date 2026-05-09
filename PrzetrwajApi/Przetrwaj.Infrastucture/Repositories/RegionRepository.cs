using Dapper;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Exceptions.Regions;
using Przetrwaj.Domain.Helpers;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Infrastucture.Context;
using System.Collections.Frozen;

namespace Przetrwaj.Infrastucture.Repositories;


public class RegionRepository : IRegionRepository
{
	private readonly NpgsqlDataSource _postgisDataSource;
	private readonly IDbContextFactory<ApplicationDbContext> _contextFactoryRO;
	private readonly ApplicationDbContext _dbContext;
	private readonly IAppCache _cache;
	private const string RegionsCacheKey = "Regions";
	private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24); // Long duration for static data
	static private readonly string _gminaRGCSql = @"
		SELECT jpt_kod_je, jpt_nazwa_
		FROM gminy 
		WHERE ST_Contains(
			geom,
			ST_Transform(ST_SetSRID(ST_MakePoint(@lon, @lat), 4326), 2180)
		) LIMIT 1;";

	public RegionRepository(IAppCache cache, IDbContextFactory<ApplicationDbContext> contextFactory, ApplicationDbContext dbContext, NpgsqlDataSource dataSource)
	{
		_cache = cache;
		_contextFactoryRO = contextFactory;
		_dbContext = dbContext;
		_postgisDataSource = dataSource;
	}

	private async Task<GminaRGCResult?> GetGminaByCoordinatesAsync(LatLong latLong)
	{
		using var connection = await _postgisDataSource.OpenConnectionAsync();
		return await connection.QueryFirstOrDefaultAsync<GminaRGCResult>(_gminaRGCSql, new { lon = latLong.Long, lat = latLong.Lat });
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
			regions.CompundDict = list.ToFrozenDictionary(r => r.Id);
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
		if (id < 0 || id > 100_00_000) return null; // filter out invalid ids
		var regionId = RegionCompoundHelper.UnifyRegionId(id);
		// Don't go to DB. Use the cached list.
		var allRegions = await GetAllAsync(ct);
		return allRegions.CompundDict.GetValueOrDefault(regionId);
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

	public async Task<IRegionInfo?> RegionFromLocationAsync(LatLong location, CancellationToken ct = default)
	{
		var gmina = await GetGminaByCoordinatesAsync(location);
		if (gmina is null) throw new LocationNotInPolandException(location);
		var res = await GetByIdAsync(gmina.Jpt_kod_je, ct);
		if (res is null) throw new RegionNotFoundException(gmina.Jpt_kod_je);
		return res;
	}
}