using Przetrwaj.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Dtos;

public class UserGeneralDto
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
	public RegionOnlyDto? Region { get; set; }


	public static explicit operator UserGeneralDto(AppUser registeredUser)
	{
		return new UserGeneralDto
		{
			Id = registeredUser.Id,
			Name = registeredUser.Name ?? "",
			//Role = string.Join(", ", registeredUser.clai.ToList()),
			Surname = registeredUser.Surname ?? "",
			Region = registeredUser.IdRegionNavigation == null ? null : (RegionOnlyDto)registeredUser.IdRegionNavigation,
		};
	}
}
