using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Users;

public class BanUserCommand
{
	[Required]
	public required string UserIdOrEmail { get; set; }
	[Required]
	public required string Reason { get; set; }
}
