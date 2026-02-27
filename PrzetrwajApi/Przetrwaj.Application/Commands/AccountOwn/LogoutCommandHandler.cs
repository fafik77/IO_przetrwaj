using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
	private readonly IJwtService _jwtService;

	public LogoutCommandHandler(IJwtService jwtService)
	{
		_jwtService = jwtService;
	}

	public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
	{
		await _jwtService.BlockTokenAsync(request.UserId, request.TokenId);
	}
}
