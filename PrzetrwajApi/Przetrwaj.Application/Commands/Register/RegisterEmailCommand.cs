using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;

namespace Przetrwaj.Application.Commands.Register
{
	public class RegisterEmailCommand : ICommand<RegisteredUserDto>
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string Name { get; set; }
		public required string Surname { get; set; }
	}
}
