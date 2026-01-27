using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Categories;

namespace Przetrwaj.Application.Commands.Categories.Dangers;

public class UpdateDangerCategoryCommandHandler : ICommandHandler<UpdateDangerCategoryCommand>
{
	private readonly ICategoryRepository _categoryRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateDangerCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
	{
		_categoryRepository = categoryRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task Handle(UpdateDangerCategoryCommand request, CancellationToken cancellationToken)
	{
		var cat = await _categoryRepository.GetDangerByIdAsync(request.Id, cancellationToken);
		if (cat is null) throw new CategoryNotFoundException(request.Id);
		cat.CategoryIcon = request.CategoryIcon;
		cat.Name = request.Name;
		_categoryRepository.Update(cat);
		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
	}
}
