using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Posts;

public class AddCommentCommand
{
	[Required]
	[MaxLength(1000)]
	public required string Comment { get; set; }
}
