using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories.Resources;

public class UpdateResourceCategoryCommand : ICommand
{
	[Required]
	public int Id { get; set; }
	[Required]
	[MinLength(3)]
	[MaxLength(100)]
	public required string Name { get; set; }
	public string? CategoryIcon { get; set; }
}
