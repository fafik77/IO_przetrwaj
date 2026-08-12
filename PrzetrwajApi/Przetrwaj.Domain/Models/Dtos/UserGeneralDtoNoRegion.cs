using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class UserGeneralDtoNoRegion
{
	public required string Id { get; set; }
	public required string Name { get; set; }
	public required string Surname { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }
	public DateTimeOffset? BanDate { get; set; }

	public static UserGeneralDtoNoRegion? Map(AppUser? registeredUser)
	{
		return registeredUser is null ? null : new UserGeneralDtoNoRegion
		{
			Id = registeredUser.Id,
			Name = registeredUser.Name ?? "",
			Surname = registeredUser.Surname ?? "",
			RegistrationDate = registeredUser.RegistrationDate,
			BanDate = registeredUser.BanDate,
		};
	}
}
