using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Dtos;

public class RegisteredUserDto
{
	public string Id { get; set; }
	public string Email { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
	public string Role { get; set; }


	public static explicit operator RegisteredUserDto(AppUser registeredUser)
	{
		return new RegisteredUserDto
		{
			Id = registeredUser.Id,
			Email = registeredUser.Email,
			Name = registeredUser.Name ?? "",
			//Role = string.Join(", ", registeredUser.clai.ToList()),
			Surname = registeredUser.Surname ?? "",
		};
	}
}
