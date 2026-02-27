using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Infrastucture.Repositories;

public class UserJwtRefreshRepository : IUserJwtRefreshRepository
{
	public Task<UserJwtRefresh> AddAsync(UserJwtRefresh userJwtRefresh, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public void Delete(UserJwtRefresh userJwtRefresh)
	{
		throw new NotImplementedException();
	}

	public void Delete(string userId, string tokenId)
	{
		throw new NotImplementedException();
	}

	public void DeleteAll(string userId)
	{
		throw new NotImplementedException();
	}

	public Task<IList<UserJwtRefresh>> GetByIdAsync(string userId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task<UserJwtRefresh> GetByIdAsync(string userId, string tokenId, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public void Update(UserJwtRefresh userJwtRefresh)
	{
		throw new NotImplementedException();
	}
}