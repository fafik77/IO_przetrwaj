using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Models;

namespace Przetrwaj.Application.Commands.Register
{
	public class RegisterEmailCommand : RegisterEmailInfo, ICommand<UserWithPersonalDataDto>
	{
	}
}
