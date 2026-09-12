using Przetrwaj.Application.Configuration.Commands;

namespace Przetrwaj.Application.Commands.Posts;

public record UpdatePostInternalCommand : ICommand
{
    public required UpdatePostCommand UpdatePost { get; set; }
    public required string Id { get; set; }
}
