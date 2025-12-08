using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

public class CategoryRepository : ICategoryRepository
{
	private readonly ApplicationDbContext _dbContext;

	public CategoryRepository(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken)
	{
		await _dbContext.Categories.AddAsync(category, cancellationToken);
		//await _dbContext.SaveChangesAsync(cancellationToken);
		return category;
	}

	public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
	{
		return await _dbContext.Categories
			.AsNoTracking() //only for multiple entries we use this
			.ToListAsync(cancellationToken);
	}

	public async Task<Category?> GetByIdAsync(int idCategory, CancellationToken cancellationToken)
	{
		return await _dbContext.Categories //for single entries we want with tracking
			.FirstOrDefaultAsync(c => c.IdCategory == idCategory, cancellationToken);
	}
}
