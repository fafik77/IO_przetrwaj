using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface IUserRepository
{
	Task<AppUser> RegisterUserByEmailAsync(string email, string password, string name, string surname);

}
