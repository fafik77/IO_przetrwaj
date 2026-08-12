using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Categories.Dangers;

internal class CreateDangerCategoriesCommandHandler : ICommandHandler<CreateDangerCategoriesCommand, IEnumerable<CategoryDto>>
{
	private readonly ICategoryRepository _repo;
	private readonly IUnitOfWork _uow;

	public CreateDangerCategoriesCommandHandler(ICategoryRepository repo, IUnitOfWork uow)
	{
		_repo = repo;
		_uow = uow;
	}

	public async Task<IEnumerable<CategoryDto>> Handle(CreateDangerCategoriesCommand request, CancellationToken cancellationToken)
	{
		var categories = (List<CategoryDanger>)request;
		foreach (var category in categories)
		{
			await _repo.AddAsync(category, cancellationToken);
		}
		try
		{
			await _uow.SaveChangesAsync(cancellationToken);//this could throw
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
		return categories.Select(c => CategoryDto.Map(c)!).ToList();
	}
}
