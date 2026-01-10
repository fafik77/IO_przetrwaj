using LazyCache;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;


public class CategoryRepository : ICategoryRepository
{
	private readonly ApplicationDbContext _db;
	private readonly IAppCache _cache;
	private const string CategoryCacheKey = "Categories";
	private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24);

	public CategoryRepository(ApplicationDbContext db, IAppCache cache)
	{
		_db = db;
		_cache = cache;
	}

	// Helper to get the master list
	private async Task<IEnumerable<Category>> GetAllInternalAsync(CancellationToken ct)
	{
		return await _cache.GetOrAddAsync(CategoryCacheKey, async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
			// Fetching the base type gets all derived types (TPH)
			return await _db.Categories.AsNoTracking().ToListAsync(ct);
		});
	}

	public async Task<IEnumerable<CategoryDanger>> GetDangersAsync(CancellationToken ct)
	{
		var all = await GetAllInternalAsync(ct);
		// OfType filters and casts the objects in-memory
		return all.OfType<CategoryDanger>();
	}

	public async Task<IEnumerable<CategoryResource>> GetResourcesAsync(CancellationToken ct)
	{
		var all = await GetAllInternalAsync(ct);
		return all.OfType<CategoryResource>();
	}

	public async Task<CategoryDanger?> GetDangerByIdAsync(int id, CancellationToken ct)
	{
		var list = await GetDangersAsync(ct);
		return list.FirstOrDefault(c => c.IdCategory == id);
	}

	public async Task<CategoryResource?> GetResourceByIdAsync(int id, CancellationToken ct)
	{
		var list = await GetResourcesAsync(ct);
		return list.FirstOrDefault(c => c.IdCategory == id);
	}


	public async Task AddAsync(Category item, CancellationToken cancellationToken)
	{
		await _db.Categories.AddAsync(item, cancellationToken);
		_cache.Remove(CategoryCacheKey); // Invalidate cache
	}

	public void Delete(Category category)
	{
		_db.Categories.Remove(category);
		_cache.Remove(CategoryCacheKey); // Invalidate cache
	}

	public void Update(Category item)
	{
		_db.Categories.Update(item);
		_cache.Remove(CategoryCacheKey); // Invalidate cache
	}
}