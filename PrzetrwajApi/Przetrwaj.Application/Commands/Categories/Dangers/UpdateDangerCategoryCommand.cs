using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories.Dangers;

public class UpdateDangerCategoryCommand : ICommand
{
	[Required]
	public int Id { get; set; }
	public required AddUpdateCategory Category { get; set; }
}
