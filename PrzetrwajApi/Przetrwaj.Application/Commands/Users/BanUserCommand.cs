using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Users;

public class BanUserCommand : ICommand<UserWithPersonalDataDto>
{
	[Required]
	public required string UserIdOrEmail { get; set; }
	[Required]
	public required string ModeratorId { get; set; }
	[Required]
	public required string Reason { get; set; }
}
