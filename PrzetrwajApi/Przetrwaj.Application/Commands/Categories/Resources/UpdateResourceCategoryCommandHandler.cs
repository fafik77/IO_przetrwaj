using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Categories;

namespace Przetrwaj.Application.Commands.Categories.Resources;

public class UpdateResourceCategoryCommandHandler : ICommandHandler<UpdateResourceCategoryCommand>
{
	private readonly ICategoryRepository _categoryRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateResourceCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
	{
		_categoryRepository = categoryRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task Handle(UpdateResourceCategoryCommand request, CancellationToken cancellationToken)
	{
		var cat = await _categoryRepository.GetResourceByIdAsync(request.Id, cancellationToken);
		if (cat is null) throw new CategoryNotFoundException(request.Id);
		cat.CategoryIcon = request.CategoryIcon;
		cat.Name = request.Name;
		_categoryRepository.Update(cat);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
	}
}
