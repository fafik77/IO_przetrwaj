using System.ComponentModel.DataAnnotations;
namespace PrzetrwajPL.Requests
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "Email jest wymagany")]
		[EmailAddress(ErrorMessage = "Niepoprawny format email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Hasło jest wymagane")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
