using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Posts;

public class MarkPostAsInactiveCommand : ICommand
{
	[Required]
	public required string PostId { get; set; }
}
