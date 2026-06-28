using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Categories.Resources;

public class CreateResourceCategoryCommand : AddUpdateCategory, ICommand<CategoryDto>
{
	static public implicit operator CategoryResource(CreateResourceCategoryCommand request)
	{
		return new CategoryResource
		{
			Name = request.Name,
			CategoryIcon = request.CategoryIcon,
			Impediments = request.Impediments ?? 0
		};
	}
}
