using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class UpdateAccountInternallCommand : ICommand<UserWithPersonalDataDto>
{
	public required UpdateAccountCommand Update { get; set; }
	public required string UserId { get; set; }
}
