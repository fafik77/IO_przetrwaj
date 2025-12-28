using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Confirm;

public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, UserWithPersonalDataDto>
{
	private readonly IAuthService _authService;

	public ConfirmEmailCommandHandler(IAuthService authService)
	{
		_authService = authService;
	}

	public async Task<UserWithPersonalDataDto> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
	{
		var res = await _authService.ConfirmEmailAsync(request.UserId, request.Code);
		return (UserWithPersonalDataDto)res;
	}
}
