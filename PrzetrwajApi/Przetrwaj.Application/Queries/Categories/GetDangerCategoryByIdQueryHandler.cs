using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Quaries.Categories;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

internal class GetDangerCategoryByIdQueryHandler
    : IQueryHandler<GetDangerCategoryByIdQuery, CategoryDto?>
{
    private readonly ICategoryRepository _repo;
    public GetDangerCategoryByIdQueryHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<CategoryDto?> Handle(GetDangerCategoryByIdQuery q, CancellationToken ct)
    {
        var e = await _repo.GetDangerByIdAsync(q.IdCategory, ct);
        return e is null ? null : (CategoryDto?)e;
    }
}
