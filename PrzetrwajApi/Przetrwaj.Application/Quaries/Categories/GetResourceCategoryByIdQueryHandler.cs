using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Quaries.Categories;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

internal class GetResourceCategoryByIdQueryHandler
    : IQueryHandler<GetResourceCategoryByIdQuery, CategoryDto?>
{
    private readonly ICategoryRepository _repo;
    public GetResourceCategoryByIdQueryHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<CategoryDto?> Handle(GetResourceCategoryByIdQuery q, CancellationToken ct)
    {
        var e = await _repo.GetResourceByIdAsync(q.IdCategory, ct);
        return e is null ? null : (CategoryDto)e;
    }
}