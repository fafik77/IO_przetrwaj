using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
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

	public async Task<AppUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		id = id.ToLower();
		var res = await _dbContext.Users
			.Include(u => u.IdRegionNavigation)
			.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
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

	public async Task<IEnumerable<ModeratorPendingStatus>> GetModPendingUsersROAsync(CancellationToken cancellationToken = default)
	{
		var users = await _dbContext.Users
			.AsNoTracking()
			.Where(u => u.ModeratorRolePending && u.EmailConfirmed && !u.Banned && u.ModeratorSince == null)
			//.Include(u => u.IdRegionNavigation)
			.Select(u => new ModeratorPendingStatus
			{
				Email = u.Email!,
				Id = u.Id,
				RegionId = u.IdRegion,
				RegionName = u.IdRegionNavigation.Name,
				Surname = u.Surname,
				Name = u.Name,
			})
			.ToListAsync(cancellationToken);
		return users;
	}
}
