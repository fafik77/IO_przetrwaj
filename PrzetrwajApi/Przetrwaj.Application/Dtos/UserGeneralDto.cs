using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Dtos;

public class UserGeneralDto
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }
	public DateTimeOffset? BanDate { get; set; }


	public static explicit operator UserGeneralDto?(AppUser registeredUser)
	{
		return registeredUser is null ? null : new UserGeneralDto
		{
			Id = registeredUser.Id,
			Name = registeredUser.Name ?? "",
			//Role = string.Join(", ", registeredUser.clai.ToList()),
			Surname = registeredUser.Surname ?? "",
			Region = (RegionOnlyDto?)registeredUser.IdRegionNavigation,
			RegistrationDate = registeredUser.RegistrationDate,
			BanDate = registeredUser.BanDate,
		};
	}
}
