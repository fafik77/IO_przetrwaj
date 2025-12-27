using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models;

namespace Przetrwaj.Domain.Abstractions;

public interface IAuthService
{
	Task<AppUser> RegisterUserByEmailAsync(RegisterEmailInfo register);
	Task<AppUser> LoginUserByEmailAsync(string email, string password);
	Task<AppUser> ConfirmEmailAsync(string userId, string code);
	Task<AppUser> GetUserDetailsAsync(string userIdEmail);
}
