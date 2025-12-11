using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface ICategoryRepository
{

    Task<CategoryDanger> AddDangerAsync(string name, CancellationToken ct);
    Task<CategoryResource> AddResourceAsync(string name, CancellationToken ct);

    Task<IReadOnlyList<CategoryDanger>> GetDangersAsync(CancellationToken ct);
    Task<IReadOnlyList<CategoryResource>> GetResourcesAsync(CancellationToken ct);

    Task<CategoryDanger?> GetDangerByIdAsync(int idCategory, CancellationToken ct);
    Task<CategoryResource?> GetResourceByIdAsync(int idCategory, CancellationToken ct);

    Task<bool> DeleteDangerByIdAsync(int idCategory, CancellationToken ct);
    Task<bool> DeleteResourceByIdAsync(int idCategory, CancellationToken ct);
}
