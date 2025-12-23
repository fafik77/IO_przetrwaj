using System.Diagnostics.CodeAnalysis;

namespace Przetrwaj.Domain.Models.Dtos;

public class VoteDto
{
	[AllowNull]
	public bool? IsUpvote { get; set; } = null;

	public VoteDto(bool? isUpvote)
	{
		IsUpvote = isUpvote;
	}
}
