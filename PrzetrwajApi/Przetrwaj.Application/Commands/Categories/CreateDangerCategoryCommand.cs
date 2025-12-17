using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories;

public class CreateDangerCategoryCommand : ICommand<CategoryDto>
{
	[Required]
	[MinLength(3)]
	[MaxLength(100)]
	public required string Name { get; set; }
}
