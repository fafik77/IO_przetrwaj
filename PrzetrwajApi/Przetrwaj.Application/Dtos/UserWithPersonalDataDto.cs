using Przetrwaj.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Dtos;

public class UserWithPersonalDataDto
{
	public string Id { get; set; }
	[EmailAddress]
	public string? Email { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public string? Role { get; set; }
	public bool Banned { get; set; }
	public bool TwoFactorEnabled { get; set; }


	public static explicit operator UserWithPersonalDataDto(AppUser registeredUser)
	{
		return new UserWithPersonalDataDto
		{
			Id = registeredUser.Id,
			Email = registeredUser.Email,
			Name = registeredUser.Name ?? "",
			//Role = string.Join(", ", registeredUser.clai.ToList()),
			Surname = registeredUser.Surname ?? "",
			Region = registeredUser.IdRegionNavigation == null ? null : (RegionOnlyDto)registeredUser.IdRegionNavigation,
			Banned = registeredUser.Banned,
			TwoFactorEnabled = registeredUser.TwoFactorEnabled,
		};
	}
}
