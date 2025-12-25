using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

public class UserRepository : IUserRepository
{
	private readonly ApplicationDbContext _dbContext;
	private readonly UserManager<AppUser> _userManager;

	public UserRepository(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
	{
		_dbContext = dbContext;
		_userManager = userManager;
	}


	public async Task<IEnumerable<AppUser>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var res = await _dbContext.Users.AsNoTracking().ToListAsync();
		return res;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons", Justification = "Wrong hint, Postgres DB does not support that")]
	public async Task<AppUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		var res = await _dbContext.Users
			.Include(u => u.IdRegionNavigation)
			.FirstOrDefaultAsync(u => u.Id == id.ToLower(), cancellationToken);
		return res;
	}

	public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var normEmail = _userManager.NormalizeEmail(email);
		var res = await _dbContext.Users
			.Include(u => u.IdRegionNavigation)
			.FirstOrDefaultAsync(u => u.NormalizedEmail == normEmail, cancellationToken);
		return res;
	}
}
