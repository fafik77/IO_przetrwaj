using Przetrwaj.Domain.Abstractions._base;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface ICategoryRepository
{
	Task<Category> AddAsync(Category category, CancellationToken ct);

	Task<IEnumerable<CategoryDanger>> GetDangersAsync(CancellationToken ct);
	Task<IEnumerable<CategoryResource>> GetResourcesAsync(CancellationToken ct);

	Task<CategoryDanger?> GetDangerByIdAsync(int idCategory, CancellationToken ct);
	Task<CategoryResource?> GetResourceByIdAsync(int idCategory, CancellationToken ct);

	void Delete(Category category);
}
