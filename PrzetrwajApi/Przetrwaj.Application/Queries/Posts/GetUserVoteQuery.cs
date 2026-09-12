using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Posts;

public record GetUserVoteQuery : IQuery<VoteDto>
{
    public required string PostId { get; set; }
}
