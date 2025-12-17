using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetResourcesCategoriesQueryHandler
	: IQueryHandler<GetResourcesCategoriesQuery, IEnumerable<CategoryDto>>
{
	private readonly ICategoryRepository _repo;
	public GetResourcesCategoriesQueryHandler(ICategoryRepository repo) => _repo = repo;

	public async Task<IEnumerable<CategoryDto>> Handle(GetResourcesCategoriesQuery request, CancellationToken ct)
	{
		var list = await _repo.GetResourcesAsync(ct);
		return list.Select(c => (CategoryDto)c).ToList();
	}
}
