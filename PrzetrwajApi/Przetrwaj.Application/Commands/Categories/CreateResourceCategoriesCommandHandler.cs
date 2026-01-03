using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateResourceCategoriesCommandHandler : ICommandHandler<CreateResourceCategoriesCommand, IEnumerable<CategoryDto>>
{
	private readonly ICategoryRepository _repo;
	private readonly IUnitOfWork _uow;

	public CreateResourceCategoriesCommandHandler(ICategoryRepository repo, IUnitOfWork uow)
	{
		_repo = repo;
		_uow = uow;
	}

	public async Task<IEnumerable<CategoryDto>> Handle(CreateResourceCategoriesCommand request, CancellationToken cancellationToken)
	{
		var categories = (List<CategoryResource>)request;
		foreach (var category in categories)
		{
			await _repo.AddAsync(category, cancellationToken);
		}
		await _uow.SaveChangesAsync(cancellationToken); //this could throw
		return categories.Select(c => (CategoryDto)c!).ToList();
	}
}
