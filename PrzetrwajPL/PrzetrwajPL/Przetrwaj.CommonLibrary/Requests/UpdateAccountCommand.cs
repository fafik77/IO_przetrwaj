using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.CommonLibrary.Requests;

public class UpdateAccountCommand
{
	[MaxLength(64)]
	public string? Name { get; set; }
	[MaxLength(64)]
	public string? Surname { get; set; }
	[EmailAddress(ErrorMessage = "Niepoprawny format email")]
	public int? GminaId { get; set; }
	public int? Impediments { get; set; }




	// email
	public string? Email { get; set; }

	[PasswordPropertyText]
	public string? OldPassword { get; set; }

	// password (with OldPassword)
	[PasswordPropertyText]
	public string? NewPassword { get; set; }

	[PasswordPropertyText]
	[Compare(otherProperty: nameof(NewPassword), ErrorMessage = "Hasła nie są takie same !")]
	public string? ConfirmPassword { get; set; }
}
