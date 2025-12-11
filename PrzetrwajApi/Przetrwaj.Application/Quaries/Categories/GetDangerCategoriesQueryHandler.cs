using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetDangerCategoriesQueryHandler
    : IQueryHandler<GetDangerCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _repo;
    public GetDangerCategoriesQueryHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetDangerCategoriesQuery request, CancellationToken ct)
    {
        var list = await _repo.GetDangersAsync(ct);
        return list.Select(c => (CategoryDto)c).ToList();
    }
}
