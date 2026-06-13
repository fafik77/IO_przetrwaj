using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class AddCommentCommand
{
	[Required]
	[MaxLength(1000)]
	public required string Comment { get; set; }
}
