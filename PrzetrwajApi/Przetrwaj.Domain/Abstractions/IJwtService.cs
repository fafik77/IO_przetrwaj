using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Abstractions;

public interface IJwtService
{
	Task<JwtTokenDto> GenerateTokenAsync(UserWithPersonalDataDto user);

	Task BlockTokenAsync(string userId, string tokenId);
	Task RefreshTokenAsync(string userId, string tokenId, string refreshToken);
}
