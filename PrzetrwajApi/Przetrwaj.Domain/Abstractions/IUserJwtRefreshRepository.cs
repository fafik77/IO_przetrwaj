using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface IUserJwtRefreshRepository
{
	Task<IList<UserJwtRefresh>> GetByIdAsync(string userId, CancellationToken ct);
	Task<UserJwtRefresh?> GetByIdAsync(string userId, string tokenId, CancellationToken ct);
	Task<UserJwtRefresh> AddAsync(UserJwtRefresh userJwtRefresh, CancellationToken ct);
	void Update(UserJwtRefresh userJwtRefresh);
	Task DeleteAsync(UserJwtRefresh userJwtRefresh, CancellationToken ct);
	Task DeleteAsync(string userId, string tokenId, CancellationToken ct);
	Task DeleteEntriesOlderThanAsync(DateTimeOffset dateTimeOffset, CancellationToken ct);
	Task DeleteAllAsync(string userId, CancellationToken ct);
}