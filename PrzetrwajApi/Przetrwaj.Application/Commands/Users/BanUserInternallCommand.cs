using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Users;

public class BanUserInternallCommand : BanUserCommand, ICommand<UserWithPersonalDataDto>
{
	[Required]
	public required string ModeratorId { get; set; }
}
