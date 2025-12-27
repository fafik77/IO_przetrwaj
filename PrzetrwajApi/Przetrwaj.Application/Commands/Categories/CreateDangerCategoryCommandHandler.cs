using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Categories;

internal class CreateDangerCategoryCommandHandler
	: ICommandHandler<CreateDangerCategoryCommand, CategoryDto>
{
	private readonly ICategoryRepository _repo;
	private readonly IUnitOfWork _uow;

	public CreateDangerCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow)
	{
		_repo = repo; _uow = uow;
	}

	public async Task<CategoryDto> Handle(CreateDangerCategoryCommand request, CancellationToken ct)
	{
		var cat = new CategoryDanger { Name = request.Name };
		var e = await _repo.AddAsync(cat, ct);
		await _uow.SaveChangesAsync(ct);
		return (CategoryDto)e!;
	}
}
