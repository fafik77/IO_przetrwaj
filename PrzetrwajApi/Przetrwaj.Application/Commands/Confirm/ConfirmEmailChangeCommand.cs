using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Confirm;

public record ConfirmEmailChangeCommand : ConfirmEmailChangeInfo, ICommand<UserWithPersonalDataDto>
{ }
