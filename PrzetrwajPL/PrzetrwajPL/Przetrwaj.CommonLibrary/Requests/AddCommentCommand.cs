using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class AddCommentCommand
{
	[Required]
	[MaxLength(1000)]
	public required string Comment { get; set; }
}
