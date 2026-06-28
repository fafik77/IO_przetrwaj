using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories;

public class AddUpdateCategory
{
	[Required]
	[MinLength(3)]
	[MaxLength(100)]
	public required string Name { get; set; }
	public string? CategoryIcon { get; set; }
	public int? Impediments { get; set; } = 0;
}
