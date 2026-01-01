using Przetrwaj.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Przetrwaj.Domain.Models.Dtos;

public class VoteDto
{
	[AllowNull]
	public bool? IsUpvoteOrNull { get; set; } = null;

	public VoteDto(bool? isUpvote)
	{
		IsUpvoteOrNull = isUpvote;
	}

	public static explicit operator VoteDto(Vote? vote)
	{
		return new VoteDto
		(
			vote?.IsUpvote ?? null
		);
	}
}
