using Przetrwaj.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Models.Dtos;

public class UserWithPersonalDataDto
{
	public required string Id { get; set; }
	[EmailAddress]
	public string? Email { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public string? Role { get; set; }
	public bool TwoFactorEnabled { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }

	public bool Banned { get; set; }
	/// <summary>
	/// You have to include `BannedBy` yourself when making a Dto
	/// </summary>
	public BanInfo? BanInfo { get; set; }
	//public string? BanReason { get; set; }
	//public DateTimeOffset? BanDate { get; set; }
	//public UserGeneralDto? BannedBy { get; set; }


	public static explicit operator UserWithPersonalDataDto(AppUser registeredUser)
	{
		return new UserWithPersonalDataDto
		{
			Id = registeredUser.Id,
			Email = registeredUser.Email,
			Name = registeredUser.Name ?? "",
			//Role = string.Join(", ", registeredUser.clai.ToList()),
			Surname = registeredUser.Surname ?? "",
			Region = (RegionOnlyDto?)registeredUser.IdRegionNavigation,
			Banned = registeredUser.Banned,
			BanInfo = registeredUser.Banned ? new BanInfo
			{
				Banned = true,
				BanReason = registeredUser.BanReason ?? string.Empty,
				BanDate = registeredUser.BanDate,
				BannedById = registeredUser.BannedById ?? string.Empty,
				BannedBy = null,
			} : null,
			TwoFactorEnabled = registeredUser.TwoFactorEnabled,
			RegistrationDate = registeredUser.RegistrationDate,
		};
	}
}
