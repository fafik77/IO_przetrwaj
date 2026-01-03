using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateResourceCategoryCommandHandler : ICommandHandler<CreateResourceCategoryCommand, CategoryDto>
{
	private readonly ICategoryRepository _repo;
	private readonly IUnitOfWork _uow;

	public CreateResourceCategoryCommandHandler(ICategoryRepository repo, IUnitOfWork uow)
	{
		_repo = repo; _uow = uow;
	}

	public async Task<CategoryDto> Handle(CreateResourceCategoryCommand request, CancellationToken ct)
	{
		var category = (CategoryResource)request;
		await _repo.AddAsync(category, ct);
		await _uow.SaveChangesAsync(ct);    //this could throw
		return (CategoryDto)category!;
	}
}
