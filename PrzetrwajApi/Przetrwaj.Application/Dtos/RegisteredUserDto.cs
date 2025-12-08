using Przetrwaj.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Dtos;

public class RegisteredUserDto
{
	public string Id { get; set; }
	[EmailAddress]
	public string Email { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
	public int IdRegion { get; set; }
	public string RegionName { get; set; }
	public string Role { get; set; }
	public bool Banned { get; set; }
	public bool TwoFactorEnabled { get; set; }


	public static explicit operator RegisteredUserDto(AppUser registeredUser)
	{
		return new RegisteredUserDto
		{
			Id = registeredUser.Id,
			Email = registeredUser.Email,
			Name = registeredUser.Name ?? "",
			//Role = string.Join(", ", registeredUser.clai.ToList()),
			Surname = registeredUser.Surname ?? "",
			IdRegion = registeredUser.IdRegion,
			RegionName = registeredUser.IdRegionNavigation.Name,
			Banned = registeredUser.Banned,
			TwoFactorEnabled = registeredUser.TwoFactorEnabled,
		};
	}
}
