using System.Diagnostics.CodeAnalysis;

namespace Przetrwaj.CommonLibrary.Models;

public class VoteDto
{
	[AllowNull]
	public bool? IsUpvote { get; set; } = null;
}
