namespace Przetrwaj.Domain.Models
{
	public class RegisterEmailInfo
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string Name { get; set; }
		public required string Surname { get; set; }
		public int? IdRegion { get; set; }
	}
}
