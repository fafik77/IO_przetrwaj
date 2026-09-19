using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetAllCategoriesQueryHandler(ICategoryRepository repo)
        : IQueryHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _repo = repo;

    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
    {
        var list = await _repo.GetAllCategoriesAsync(ct);
        return list.Select(c => CategoryDto.Map(c)!).ToList();
    }
}
