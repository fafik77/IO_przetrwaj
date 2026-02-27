using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface IUserJwtRefreshRepository
{
	Task<IList<UserJwtRefresh>> GetByIdAsync(string userId, CancellationToken ct);
	Task<UserJwtRefresh> GetByIdAsync(string userId, string tokenId, CancellationToken ct);
	Task<UserJwtRefresh> AddAsync(UserJwtRefresh userJwtRefresh, CancellationToken ct);
	void Update(UserJwtRefresh userJwtRefresh);
	void Delete(UserJwtRefresh userJwtRefresh);
	void Delete(string userId, string tokenId);
	void DeleteAll(string userId);
}