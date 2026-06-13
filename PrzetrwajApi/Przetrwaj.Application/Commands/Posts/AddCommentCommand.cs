using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Posts;

public record AddCommentCommand
{
	[Required]
	[MaxLength(1000)]
	public required string Comment { get; set; }
}
