using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class UpdateAccountInternallCommand : UpdateAccountCommand, ICommand<UserWithPersonalDataDto>
{
	public required string UserId { get; set; }
}
