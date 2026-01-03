using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Categories;

internal class CreateDangerCategoryCommandHandler : ICommandHandler<CreateDangerCategoryCommand, CategoryDto>
{
	private readonly ICategoryRepository _repo;
	private readonly IUnitOfWork _uow;

	public CreateDangerCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow)
	{
		_repo = repo; _uow = uow;
	}

	public async Task<CategoryDto> Handle(CreateDangerCategoryCommand request, CancellationToken ct)
	{
		var category = (CategoryDanger)request;
		await _repo.AddAsync(category, ct);
		await _uow.SaveChangesAsync(ct);
		return (CategoryDto)category!;
	}
}
