using LazyCache;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

public class ImpedimentsRepository : IImpedimentsRepository
{
	private readonly ApplicationDbContext _db;
	private readonly IAppCache _cache;
	/// <summary>
	/// Impediments
	/// </summary>
	private readonly string ImpedimentsCacheKey = "Impediments";
	private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24);

	public ImpedimentsRepository(ApplicationDbContext db, IAppCache cache)
	{
		_db = db;
		_cache = cache;
	}

	public async Task AddAsync(Impediment item, CancellationToken cancellationToken = default)
	{
		await _db.Impediments.AddAsync(item, cancellationToken);
		_cache.Remove(ImpedimentsCacheKey); // Invalidate cache
	}

	public void Delete(Impediment item)
	{
		_db.Impediments.Remove(item);
		_cache.Remove(ImpedimentsCacheKey); // Invalidate cache
	}

	public async Task<IDictionary<short, string>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await GetAllInternalAsync(cancellationToken);
	}
	private async Task<IDictionary<short, string>> GetAllInternalAsync(CancellationToken ct = default)
	{
		return await _cache.GetOrAddAsync(ImpedimentsCacheKey, async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
			// Fetch from DB
			return await _db.Impediments.AsNoTracking().OrderBy(e => e.Id)
			.ToDictionaryAsync(it => it.Id, it => it.Name, ct);
		});
	}

	public async Task<Impediment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		var all = await GetAllAsync(cancellationToken);
		if (all.TryGetValue((short)id, out string name))
		{
			return new Impediment { Id = (short)id, Name = name };
		}
		return null;
	}

	public void Update(Impediment item)
	{
		_db.Impediments.Update(item);
		_cache.Remove(ImpedimentsCacheKey); // Invalidate cache
	}
}