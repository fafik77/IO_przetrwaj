using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Categories;

public class DeleteResourceCategoryCommand : ICommand<bool>
{
	[Required]
	public int IdCategory { get; set; }
}
