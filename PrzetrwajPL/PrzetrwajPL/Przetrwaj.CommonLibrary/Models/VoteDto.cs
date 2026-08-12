using System.Diagnostics.CodeAnalysis;

namespace Przetrwaj.CommonLibrary.Models;

public class VoteDto
{
	[AllowNull]
	public bool? IsUpvoteOrNull { get; set; } = null;
}
