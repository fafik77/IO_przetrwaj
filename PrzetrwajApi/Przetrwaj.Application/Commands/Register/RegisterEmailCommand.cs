using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Register;

public record RegisterEmailCommand : RegisterEmailInfo, ICommand<UserWithPersonalDataDto>
{
}
