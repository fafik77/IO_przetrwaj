using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateDangerCategoryCommand : ICommand<CategoryDto>
{
	[Required]
	public required string Name { get; set; }
}
