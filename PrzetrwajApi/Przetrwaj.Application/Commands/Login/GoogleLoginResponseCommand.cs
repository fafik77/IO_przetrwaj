using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Login;

public class GoogleLoginResponseCommand : ICommand<JwtTokenDto>
{ }
