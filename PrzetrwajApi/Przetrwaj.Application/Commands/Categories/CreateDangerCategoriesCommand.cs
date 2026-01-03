using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateDangerCategoriesCommand : ICommand<IEnumerable<CategoryDto>>
{
	[Required]
	public required IEnumerable<CreateDangerCategoryCommand> Categories { get; set; }

	static public implicit operator List<CategoryDanger>(CreateDangerCategoriesCommand request)
	{
		return request.Categories.Select(c => (CategoryDanger)c).ToList();
	}
}
