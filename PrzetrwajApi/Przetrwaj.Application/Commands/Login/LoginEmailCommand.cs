using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Login;

public class LoginEmailCommand : ICommand<UserWithPersonalDataDto>
{
	[Required]
	[EmailAddress]
	public required string Email { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public required string Password { get; set; }
}
