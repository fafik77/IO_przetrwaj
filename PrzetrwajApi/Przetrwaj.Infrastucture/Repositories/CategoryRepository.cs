using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

public class CategoryRepository : ICategoryRepository
{
	private readonly ApplicationDbContext _db;
	public CategoryRepository(ApplicationDbContext db) => _db = db;


	public async Task<Category> AddAsync(Category category, CancellationToken ct)
	{
		await _db.Categories.AddAsync(category, ct);
		return category;
	}

	public async Task<IEnumerable<CategoryDanger>> GetDangersAsync(CancellationToken ct)
	{
		var list = await _db.CategoryDangers
			.AsNoTracking()
			.ToListAsync(ct);
		return list;
	}

	public async Task<IEnumerable<CategoryResource>> GetResourcesAsync(CancellationToken ct)
	{
		var list = await _db.CategoryResources
			.AsNoTracking()
			.ToListAsync(ct);
		return list;
	}

	public Task<CategoryDanger?> GetDangerByIdAsync(int id, CancellationToken ct)
	{
		return _db.CategoryDangers
		   .FirstOrDefaultAsync(c => c.IdCategory == id, ct);
	}

	public Task<CategoryResource?> GetResourceByIdAsync(int id, CancellationToken ct)
	{
		return _db.CategoryResources
		   .FirstOrDefaultAsync(c => c.IdCategory == id, ct);
	}

	public void Delete(Category category)
	{
		_db.Categories.Remove(category);
	}
}
