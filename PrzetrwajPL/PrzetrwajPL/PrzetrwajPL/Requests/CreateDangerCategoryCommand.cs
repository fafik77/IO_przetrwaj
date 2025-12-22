using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class CreateDangerCategoryCommand
{
	[Required]
	[MinLength(3)]
	[MaxLength(100)]
	public required string Name { get; set; }
}
