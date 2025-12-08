using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;

namespace Przetrwaj.Application.Commands.Login;

public class LoginEmailCommand : ICommand<RegisteredUserDto>
{
	public required string Email { get; set; }
	public required string Password { get; set; }
}
