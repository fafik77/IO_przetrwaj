using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Login;

public class LoginEmailCommand : ICommand<JwtTokenDto>
{
	[Required]
	[EmailAddress]
	public required string Email { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public required string Password { get; set; }
}
