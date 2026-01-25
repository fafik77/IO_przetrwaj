using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class UpdateAccountCommand
{
	[MaxLength(64)]
	public string? Name { get; set; }
	[MaxLength(64)]
	public string? Surname { get; set; }
    [EmailAddress(ErrorMessage = "Niepoprawny format email")]
    public string? Email { get; set; }
	public int? IdRegion { get; set; }

	[PasswordPropertyText]
	public string NewPassword { get; set; }

	[PasswordPropertyText]
    public string OldPassword { get; set; }
}
