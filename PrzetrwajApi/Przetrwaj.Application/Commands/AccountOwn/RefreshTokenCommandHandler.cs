using Microsoft.IdentityModel.Tokens;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenInternalCommand, JwtTokenDto>
{
	private readonly IJwtService _jwtService;

	public RefreshTokenCommandHandler(IJwtService jwtService)
	{
		_jwtService = jwtService;
	}

	public async Task<JwtTokenDto> Handle(RefreshTokenInternalCommand request, CancellationToken cancellationToken)
	{
		var res = await _jwtService.RefreshTokenAsync(request.UserId, request.Jti, request.RefreshToken, cancellationToken);
		if (res is null)
			throw new SecurityTokenException("User tokens do not match");
		return res;
	}
}