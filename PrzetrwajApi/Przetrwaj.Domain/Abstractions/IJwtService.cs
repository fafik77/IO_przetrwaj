using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Abstractions;

public interface IJwtService
{
	Task<JwtTokenDto> GenerateTokenAsync(UserWithPersonalDataDto user, CancellationToken ct);

	Task BlockTokenAsync(string userId, string tokenId, CancellationToken ct);
	Task BlockAllTokenAsync(string userId, CancellationToken ct);
	Task<JwtTokenDto?> RefreshTokenAsync(string userId, string tokenId, string refreshToken, CancellationToken ct);
}
