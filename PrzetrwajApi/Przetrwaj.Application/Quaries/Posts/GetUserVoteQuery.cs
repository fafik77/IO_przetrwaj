using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetUserVoteQuery : IQuery<VoteDto>
{
	public required string UserId { get; set; }
	public required string PostId { get; set; }
}
