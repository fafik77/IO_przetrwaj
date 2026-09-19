using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Users;

public class BanUserCommand
{
	[Required]
	public required string UserIdOrEmail { get; set; }
	[Required]
    [MaxLength(500)]
    public required string Reason { get; set; }
}
