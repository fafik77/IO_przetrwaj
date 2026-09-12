using Przetrwaj.Application.Configuration.Commands;

namespace Przetrwaj.Application.Commands.Posts;

public record UpdateCommentInternalCommand : AddCommentCommand, ICommand
{
    public required string IdComment { get; set; }
}
