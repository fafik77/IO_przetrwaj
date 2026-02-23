using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Abstractions;

public interface IJwtService
{
	string GenerateToken(UserWithPersonalDataDto user);
}
