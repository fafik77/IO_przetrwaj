using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public record UpdateAccountPasswordCommand : IUpdateAccountCommand
{
	[MaxLength(64)]
	[PasswordPropertyText]
	public required string NewPassword { get; set; }

	[MaxLength(64)]
	[PasswordPropertyText]
	public string? OldPassword { get; set; }
}