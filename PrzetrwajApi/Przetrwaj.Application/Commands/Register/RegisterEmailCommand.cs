using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Register
{
	public class RegisterEmailCommand : RegisterEmailInfo, ICommand<UserWithPersonalDataDto>
	{
	}
}
