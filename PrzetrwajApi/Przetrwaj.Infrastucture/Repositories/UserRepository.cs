using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
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

	public async Task<AppUser> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		var res = await _dbContext.Users
			.Include(u => u.IdRegionNavigation)
			.FirstOrDefaultAsync(u => u.Id == id.ToLower(), cancellationToken);
		if (res == null) throw new UserNotFoundException(id);
		return res;
	}

	public async Task<AppUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var normEmail = _userManager.NormalizeEmail(email);
		var res = await _dbContext.Users
			.Include(u => u.IdRegionNavigation)
			.FirstOrDefaultAsync(u => u.NormalizedEmail == normEmail, cancellationToken);
		if (res == null) throw new UserNotFoundException(email);
		return res;
	}
}
