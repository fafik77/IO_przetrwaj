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
        Category added = request.Type switch
        {
            CategoryType.Danger => await _categoryRepository
                                            .AddDangerAsync(request.Name, cancellationToken),
            CategoryType.Resource => await _categoryRepository
                                            .AddResourceAsync(request.Name, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Type), "Unknown category type")
        };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (CategoryDto)added;
    }
}
