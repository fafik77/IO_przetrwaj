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
	public string? Role { get; set; }
	public bool TwoFactorEnabled { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }

	public bool Banned { get; set; }
	public string? BanReason { get; set; }
	public DateTimeOffset? BanDate { get; set; }
	public UserGeneralDto? BannedBy { get; set; }
}
