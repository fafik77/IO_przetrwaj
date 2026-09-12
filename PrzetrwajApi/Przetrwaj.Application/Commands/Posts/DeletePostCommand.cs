using Przetrwaj.Application.Configuration.Commands;

namespace Przetrwaj.Application.Commands.Posts;

public record DeletePostCommand : ICommand
{
    public required string Id { get; set; }
}
