using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Models;

public class RegisterEmailInfo
{
	[Required]
	[EmailAddress]
	[MaxLength(127)]
	public required string Email { get; set; }
	[Required]
	[PasswordPropertyText]
	[MaxLength(1024)]
	public required string Password { get; set; }
	[Required]
	[MaxLength(24)]
	public required string Name { get; set; }
	[Required]
	[MaxLength(24)]
	public required string Surname { get; set; }
	public int? IdRegion { get; set; }

	public bool ModeratorRole { get; set; }
}
