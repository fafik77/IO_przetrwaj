using Przetrwaj.Application.Configuration.Commands;

namespace Przetrwaj.Application.Commands.Categories.Resources;

public class UpdateResourceCategoryCommand : ICommand
{
	public int Id { get; set; }
	public required AddUpdateCategory Category { get; set; }
}
