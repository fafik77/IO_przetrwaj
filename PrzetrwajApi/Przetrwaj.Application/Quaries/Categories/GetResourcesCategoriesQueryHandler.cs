using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetResourcesCategoriesQueryHandler
    : IQueryHandler<GetResourcesCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _repo;
    public GetResourcesCategoriesQueryHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetResourcesCategoriesQuery request, CancellationToken ct)
    {
        var list = await _repo.GetResourcesAsync(ct);
        return list.Select(c => (CategoryDto)c).ToList();
    }
}
