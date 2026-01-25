using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class MakeModeratorCommand
{
	[Required]
	public required string UserIdOrEmail { get; set; }
}
