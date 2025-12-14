using System.ComponentModel.DataAnnotations;
namespace PrzetrwajPL
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "Email jest wymagany")]
		[EmailAddress(ErrorMessage = "Niepoprawny format email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Hasło jest wymagane")]
		public string Password { get; set; }
	}
}
