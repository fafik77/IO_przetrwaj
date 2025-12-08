using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Categories;

internal class CreateCategoryCommandHandler
	: ICommandHandler<CreateCategoryCommand, CategoryDto>
{
	private readonly ICategoryRepository _categoryRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateCategoryCommandHandler(
		ICategoryRepository categoryRepository,
		IUnitOfWork unitOfWork)
	{
		_categoryRepository = categoryRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
	{
		Category category = request.Type switch
		{
			CategoryType.Danger => new CategoryDanger { Name = request.Name },
			CategoryType.Resource => new CategoryResource { Name = request.Name },
			_ => throw new ArgumentOutOfRangeException(nameof(request.Type), "Unknown category type")
		};

		var added = await _categoryRepository.AddAsync(category, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return (CategoryDto)added;
	}
}
