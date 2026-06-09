namespace Przetrwaj.CommonLibrary.Models;

public class UserGeneralDtoSimpleRegion
{
	public required string Id { get; set; }
	public required string Name { get; set; }
	public required string Surname { get; set; }
	public int IdRegion { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }
	public DateTimeOffset? BanDate { get; set; }
}
