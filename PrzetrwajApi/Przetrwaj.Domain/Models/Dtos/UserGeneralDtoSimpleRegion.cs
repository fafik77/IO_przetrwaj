using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class UserGeneralDtoSimpleRegion
{
	public required string Id { get; set; }
	public required string Name { get; set; }
	public required string Surname { get; set; }
	public int IdRegion { get; set; }
	public DateTimeOffset RegistrationDate { get; set; }
	public DateTimeOffset? BanDate { get; set; }


	public static explicit operator UserGeneralDtoSimpleRegion?(AppUser? registeredUser)
	{
		return registeredUser is null ? null : new UserGeneralDtoSimpleRegion
		{
			Id = registeredUser.Id,
			Name = registeredUser.Name ?? "",
			Surname = registeredUser.Surname ?? "",
			IdRegion = registeredUser.GminaId ?? 0,
			RegistrationDate = registeredUser.RegistrationDate,
			BanDate = registeredUser.BanDate,
		};
	}
}
