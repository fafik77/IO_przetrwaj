namespace PrzetrwajPL.Models;

public class UserGeneralDto
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }
	public DateTimeOffset? BanDate { get; set; }
}
