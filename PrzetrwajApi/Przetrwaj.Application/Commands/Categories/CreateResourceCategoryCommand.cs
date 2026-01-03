using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateResourceCategoryCommand : ICommand<CategoryDto>
{
	[Required]
	[MinLength(3)]
	[MaxLength(100)]
	public required string Name { get; set; }
	public string? CategoryIcon { get; set; }

	static public implicit operator CategoryResource(CreateResourceCategoryCommand request)
	{
		return new CategoryResource
		{
			Name = request.Name,
			CategoryIcon = request.CategoryIcon,
		};
	}
}
