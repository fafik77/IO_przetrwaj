using Przetrwaj.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Models.Dtos;

public record UserWithPersonalDataDto
{
	public required string Id { get; set; }
	[EmailAddress]
	public string? Email { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public int? SubRegion { get; set; }
	public IEnumerable<string> Roles { get; set; } = [];
	public DateTimeOffset RegistrationDate { get; set; }
	public int Impediments { get; set; }

	/// <summary>
	/// You have to include `BannedBy` yourself when making a Dto
	/// </summary>
	public BanInfo? BanInfo { get; set; }


	public static explicit operator UserWithPersonalDataDto(AppUser registeredUser)
	{
		return new UserWithPersonalDataDto
		{
			Id = registeredUser.Id,
			Email = registeredUser.Email,
			Name = registeredUser.Name ?? "",
			Surname = registeredUser.Surname ?? "",
			Region = RegionOnlyDto.Map(registeredUser.RegionNavigation),
			SubRegion = registeredUser.GminaId,
			BanInfo = registeredUser.BanDate is null ? null : new BanInfo
			{
				Banned = true,
				BanReason = registeredUser.BanReason ?? string.Empty,
				BanDate = registeredUser.BanDate,
				BannedById = registeredUser.BannedById ?? string.Empty,
				BannedBy = null,
			},
			RegistrationDate = registeredUser.RegistrationDate,
			Impediments = registeredUser.Impediments,
		};
	}
}
