using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Posts;

public class AddCommentInternalCommand : AddCommentCommand, ICommand<CommentDto>
{
	public required string IdPost { get; set; }
	public required string IdAutor { get; set; }
}
