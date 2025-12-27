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
	public string? BanReason { get; set; }
	public DateTimeOffset? BanDate { get; set; }
	/// <summary>
	/// You have to include this yourself when making a Dto
	/// </summary>
	public UserGeneralDto? BannedBy { get; set; }


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
			BanReason = registeredUser.BanReason,
			BanDate = registeredUser.BanDate,
			TwoFactorEnabled = registeredUser.TwoFactorEnabled,
			RegistrationDate = registeredUser.RegistrationDate,
		};
	}
}
