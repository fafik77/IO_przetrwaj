using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Categories.Resources;

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
		try
		{
			await _uow.SaveChangesAsync(ct);    //this could throw
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
		return (CategoryDto)category!;
	}
}
