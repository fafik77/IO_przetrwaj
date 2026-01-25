using System.Diagnostics.CodeAnalysis;

namespace PrzetrwajPL.Models;

public class VoteDto
{
	[AllowNull]
	public bool? IsUpvote { get; set; } = null;
}
