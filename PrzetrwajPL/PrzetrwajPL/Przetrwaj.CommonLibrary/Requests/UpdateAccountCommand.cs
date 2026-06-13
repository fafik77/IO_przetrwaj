using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class UpdateAccountCommand
{
	[MaxLength(64)]
	public string? Name { get; set; }
	[MaxLength(64)]
	public string? Surname { get; set; }
	public int? GminaId { get; set; }
	public int? Impediments { get; set; }




	// email
	[EmailAddress(ErrorMessage = "Niepoprawny format email")]
	public string Email { get; set; } = string.Empty;

	[PasswordPropertyText]
	public string? OldPassword { get; set; }
	public string? ReturnUrl { get; set; }

	// password (with OldPassword)
	[PasswordPropertyText]
	public string? NewPassword { get; set; }

	[PasswordPropertyText]
	[Compare(otherProperty: nameof(NewPassword), ErrorMessage = "Hasła nie są takie same !")]
	public string? ConfirmPassword { get; set; }
}
