using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class MakeModeratorCommand
{
	[Required]
	public required string UserIdOrEmail { get; set; }
}
