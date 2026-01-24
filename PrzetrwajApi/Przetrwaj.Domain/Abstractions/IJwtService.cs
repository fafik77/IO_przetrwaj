using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Abstractions;

public interface IJwtService
{
	string GenerateToken(AppUser user, IList<string> roles);
	string GenerateToken(UserWithPersonalDataDto user);
}
