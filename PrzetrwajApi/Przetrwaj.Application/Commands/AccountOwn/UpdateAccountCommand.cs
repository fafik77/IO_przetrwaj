using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class UpdateAccountCommand
{
	[MaxLength(64)]
	public string? Name { get; set; }
	[MaxLength(64)]
	public string? Surname { get; set; }
	[MaxLength(64)]
	[EmailAddress]
	public string? Email { get; set; }
	public int? IdRegion { get; set; }
	[MaxLength(64)]
	[PasswordPropertyText]
	public string? NewPassword { get; set; }
	[MaxLength(64)]
	[PasswordPropertyText]
	public string? OldPassword { get; set; }
}
