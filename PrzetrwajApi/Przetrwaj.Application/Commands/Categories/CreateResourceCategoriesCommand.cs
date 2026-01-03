using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateResourceCategoriesCommand : ICommand<IEnumerable<CategoryDto>>
{
	[Required]
	public required IEnumerable<CreateResourceCategoryCommand> Categories { get; set; }

	static public implicit operator List<CategoryResource>(CreateResourceCategoriesCommand request)
	{
		return request.Categories.Select(c => (CategoryResource)c).ToList();
	}
}
