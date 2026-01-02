using Przetrwaj.Domain.Abstractions._base;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface IUserRepository : IGetsAsyncRepository<AppUser, string>
{
	public Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	public Task<IEnumerable<AppUser>> GetModPendingUsersRWAsync(CancellationToken cancellationToken = default);

	//Task<AppUser> RegisterUserByEmailAsync(RegisterEmailInfo register);
	//Task<AppUser> LoginUserByEmailAsync(string email, string password);
	//Task<AppUser> ConfirmEmailAsync(string userId, string code);
}
