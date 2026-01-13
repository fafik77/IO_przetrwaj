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
			//Role = string.Join(", ", registeredUser.clai.ToList()),
			Surname = registeredUser.Surname ?? "",
			IdRegion = registeredUser.IdRegion,
			RegistrationDate = registeredUser.RegistrationDate,
			BanDate = registeredUser.BanDate,
		};
	}
}
