using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests
{
	public class RegisterRequest
	{
		[Required(ErrorMessage = "Imie jest wymagane")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Nazwisko jest wymagane")]
		public string Surname { get; set; }

		[Required(ErrorMessage = "Email jest wymagany")]
		[EmailAddress(ErrorMessage = "Niepoprawny format email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Hasło jest wymagane")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Hasło jest wymagane")]
		[Compare(otherProperty: nameof(Password), ErrorMessage = "Hasła nie są takie same !")]
		public string ConfirmPassword { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Region jest wymagany")]
		public int IdRegion { get; set; } = -1;
		public bool ModeratorRole { get; set; } = false;
	}
}
