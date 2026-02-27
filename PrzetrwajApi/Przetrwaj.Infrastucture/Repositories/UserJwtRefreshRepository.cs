using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

public class UserJwtRefreshRepository : IUserJwtRefreshRepository
{
	private readonly ApplicationDbContext _dbContext;

	public UserJwtRefreshRepository(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<UserJwtRefresh> AddAsync(UserJwtRefresh userJwtRefresh, CancellationToken ct)
	{
		await _dbContext.UserJwtRefresh.AddAsync(userJwtRefresh, ct);
		return userJwtRefresh;
	}

	public async Task DeleteAsync(UserJwtRefresh userJwtRefresh, CancellationToken ct)
	{
		_dbContext.UserJwtRefresh
			.Remove(userJwtRefresh);
	}

	public async Task DeleteAsync(string userId, string tokenId, CancellationToken ct)
	{
		await _dbContext.UserJwtRefresh
			.Where(e => e.UserId == userId && e.Jwi == tokenId)
			.ExecuteDeleteAsync(ct);
	}

	public async Task DeleteAllAsync(string userId, CancellationToken ct)
	{
		await _dbContext.UserJwtRefresh
			.Where(e => e.UserId == userId)
			.ExecuteDeleteAsync(ct);
	}

	public async Task<IList<UserJwtRefresh>> GetByIdAsync(string userId, CancellationToken ct)
	{
		return
			await _dbContext.UserJwtRefresh
			.AsNoTracking()
			.Where(e => e.UserId == userId)
			.ToListAsync(ct);
	}

	public async Task<UserJwtRefresh?> GetByIdAsync(string userId, string tokenId, CancellationToken ct)
	{
		return
			await _dbContext.UserJwtRefresh
			.Where(e => e.UserId == userId && e.Jwi == tokenId)
			.FirstOrDefaultAsync(ct);
	}

	public void Update(UserJwtRefresh userJwtRefresh)
	{
		_dbContext.UserJwtRefresh.Update(userJwtRefresh);
	}
}