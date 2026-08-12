using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;


/// <summary>
/// Warning `Region, Name, Surname` are a sensitive information when combined
/// </summary>
public class UserGeneralDto
{
	public required string Id { get; set; }
	public required string Name { get; set; }
	public required string Surname { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }
	public DateTimeOffset? BanDate { get; set; }


	public static UserGeneralDto? Map(AppUser? registeredUser)
	{
		return registeredUser is null ? null : new UserGeneralDto
		{
			Id = registeredUser.Id,
			Name = registeredUser.Name ?? "",
			Surname = registeredUser.Surname ?? "",
			Region = RegionOnlyDto.Map(registeredUser.RegionNavigation),
			RegistrationDate = registeredUser.RegistrationDate,
			BanDate = registeredUser.BanDate,
		};
	}
}
