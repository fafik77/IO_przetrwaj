using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class CreateResourceCategoryCommand
{
	[Required]
	[MinLength(3)]
	[MaxLength(100)]
	public required string Name { get; set; }
}
