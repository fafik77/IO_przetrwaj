using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models;

namespace Przetrwaj.Domain.Abstractions;

public interface IUserRepository
{
	Task<AppUser> RegisterUserByEmailAsync(RegisterEmailInfo register);
	Task<AppUser> LoginUserByEmailAsync(string email, string password);
	Task<AppUser> ConfirmEmailAsync(string userId, string code);

}
