using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Models
{
	public class RegisterEmailInfo
	{
		[Required]
		[EmailAddress]
		public required string Email { get; set; }
		[Required]
		[PasswordPropertyText]
		public required string Password { get; set; }
		[Required]
		public required string Name { get; set; }
		[Required]
		public required string Surname { get; set; }
		public int? IdRegion { get; set; }

		public bool ModeratorRole { get; set; }
	}
}
