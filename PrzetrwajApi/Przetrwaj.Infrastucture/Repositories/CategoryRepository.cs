using LazyCache;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;
using System.Collections.Frozen;

namespace Przetrwaj.Infrastucture.Repositories;


public class CategoryRepository : ICategoryRepository
{
	private readonly ApplicationDbContext _db;
	private readonly IAppCache _cache;
	private readonly string CategoryCacheKey = "Categories";
	private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24);

	public CategoryRepository(ApplicationDbContext db, IAppCache cache)
	{
		_db = db;
		_cache = cache;
	}

	private class CategoriesMaped
	{
		public FrozenDictionary<int, CategoryDanger> Dangers { get; set; } = FrozenDictionary<int, CategoryDanger>.Empty;
		public FrozenDictionary<int, CategoryResource> Resources { get; set; } = FrozenDictionary<int, CategoryResource>.Empty;
	}

	// Helper to get the master list
	private async Task<CategoriesMaped> GetAllInternalAsync(CancellationToken ct)
	{
		return await _cache.GetOrAddAsync(CategoryCacheKey, async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
			// Fetching the base type gets all derived types (TPH)
			var res = await _db.Categories.AsNoTracking().ToListAsync(ct);
			return new CategoriesMaped
			{
				Dangers = res.OfType<CategoryDanger>().ToFrozenDictionary(e => e.IdCategory),
				Resources = res.OfType<CategoryResource>().ToFrozenDictionary(e => e.IdCategory),
			};
		});
	}

	public async Task<IEnumerable<CategoryDanger>> GetDangersAsync(CancellationToken ct)
	{
		var CategoriesMaped = await GetAllInternalAsync(ct);
		return CategoriesMaped.Dangers.Select(e => e.Value);
	}

	public async Task<IEnumerable<CategoryResource>> GetResourcesAsync(CancellationToken ct)
	{
		var CategoriesMaped = await GetAllInternalAsync(ct);
		return CategoriesMaped.Resources.Select(e => e.Value);
	}

	public async Task<CategoryDanger?> GetDangerByIdAsync(int id, CancellationToken ct)
	{
		var CategoriesMaped = await GetAllInternalAsync(ct);
		return CategoriesMaped.Dangers.GetValueOrDefault(id);
	}

	public async Task<CategoryResource?> GetResourceByIdAsync(int id, CancellationToken ct)
	{
		var CategoriesMaped = await GetAllInternalAsync(ct);
		return CategoriesMaped.Resources.GetValueOrDefault(id);
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