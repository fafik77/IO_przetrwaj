namespace Przetrwaj.Domain.Models.Dtos.Posts;

public record PostVotesStatusDto
{
	public required string Id { get; set; }
	public bool Active { get; set; }
	public long VotePositive { get; set; }
	public long VoteNegative { get; set; }
}
