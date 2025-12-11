// Przetrwaj.Infrastucture/Repositories/CategoryRepository.cs
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;
    public CategoryRepository(ApplicationDbContext db) => _db = db;

    public async Task<CategoryDanger> AddDangerAsync(string name, CancellationToken ct)
    {
        var e = new CategoryDanger { Name = name };
        await _db.AddAsync(e, ct);
        return e; // SaveChanges w UnitOfWork
    }

    public async Task<CategoryResource> AddResourceAsync(string name, CancellationToken ct)
    {
        var e = new CategoryResource { Name = name };
        await _db.AddAsync(e, ct);
        return e;
    }

    public async Task<IReadOnlyList<CategoryDanger>> GetDangersAsync(CancellationToken ct)
    {
        var list = await _db.Categories
            .OfType<CategoryDanger>()
            .AsNoTracking()
            .ToListAsync(ct);

        return list; 
    }

    public async Task<IReadOnlyList<CategoryResource>> GetResourcesAsync(CancellationToken ct)
    {
        var list = await _db.Categories
            .OfType<CategoryResource>()
            .AsNoTracking()
            .ToListAsync(ct);

        return list; 
    }

    public Task<CategoryDanger?> GetDangerByIdAsync(int id, CancellationToken ct) =>
        _db.Categories.OfType<CategoryDanger>()
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.IdCategory == id, ct);

    public Task<CategoryResource?> GetResourceByIdAsync(int id, CancellationToken ct) =>
        _db.Categories.OfType<CategoryResource>()
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.IdCategory == id, ct);

    public async Task<bool> DeleteDangerByIdAsync(int id, CancellationToken ct)
    {
        var e = await _db.Categories.OfType<CategoryDanger>()
                    .FirstOrDefaultAsync(c => c.IdCategory == id, ct);
        if (e is null) return false;
        _db.Remove(e);
        return true; // SaveChanges w UnitOfWork
    }

    public async Task<bool> DeleteResourceByIdAsync(int id, CancellationToken ct)
    {
        var e = await _db.Categories.OfType<CategoryResource>()
                    .FirstOrDefaultAsync(c => c.IdCategory == id, ct);
        if (e is null) return false;
        _db.Remove(e);
        return true;
    }
}
