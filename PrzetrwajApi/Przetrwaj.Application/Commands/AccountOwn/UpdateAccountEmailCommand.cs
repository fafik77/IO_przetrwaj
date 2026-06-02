using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public record UpdateAccountEmailCommand : IUpdateAccountCommand
{
	[MaxLength(64)]
	[EmailAddress]
	public required string Email { get; set; }

	[MaxLength(64)]
	[PasswordPropertyText]
	public required string OldPassword { get; set; }
}
