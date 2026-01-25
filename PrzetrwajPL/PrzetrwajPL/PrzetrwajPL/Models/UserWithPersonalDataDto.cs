using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Models;

public class UserWithPersonalDataDto
{
	public string Id { get; set; }
	[EmailAddress]
	public string? Email { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public IEnumerable<string> Roles { get; set; } = [];
	public bool TwoFactorEnabled { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }

	public bool Banned { get; set; }
	public BanInfo? BanInfo { get; set; }
}
