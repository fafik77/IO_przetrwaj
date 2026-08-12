using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Categories.Dangers;

public class CreateDangerCategoryCommand : AddUpdateCategory, ICommand<CategoryDto>
{
	static public implicit operator CategoryDanger(CreateDangerCategoryCommand request)
	{
		return new CategoryDanger
		{
			Name = request.Name,
			CategoryIcon = request.CategoryIcon,
			Impediments = request.Impediments ?? 0,
		};
	}
}
