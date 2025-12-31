using Przetrwaj.Application.Configuration.Commands;

namespace Przetrwaj.Application.Commands.Posts;

public class VoteOnPostCommand : ICommand
{
    public required string IdPost { get; set; }
    public required string IdUser { get; set; }
    public required bool IsUpvote { get; set; }
}
