using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Models;

namespace Przetrwaj.Application.Commands.Confirm
{
	public class ConfirmEmailCommand : ConfirmEmailInfo, ICommand<UserWithPersonalDataDto>
	{
	}
}
