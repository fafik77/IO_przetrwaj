using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class BanUserCommand
{
	[Required]
	public required string UserIdOrEmail { get; set; }
	[Required]
	public required string Reason { get; set; }
}
