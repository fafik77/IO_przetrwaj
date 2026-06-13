using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class LogoutAllSesionsCommandHandler : ICommandHandler<LogoutAllSesionsCommand>
{
	private readonly IJwtService _jwtService;

	public LogoutAllSesionsCommandHandler(IJwtService jwtService)
	{
		_jwtService = jwtService;
	}

	public async Task Handle(LogoutAllSesionsCommand request, CancellationToken cancellationToken)
	{
		await _jwtService.BlockAllTokenAsync(request.UserId, cancellationToken);
	}
}

