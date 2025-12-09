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
	private readonly SignInManager<AppUser> _signInManager;
	private readonly IEmailSender _emailSender;
	private readonly IUrlHelper _urlHelper;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public UserRepository(ApplicationDbContext dbContext, UserManager<AppUser> userManager, IEmailSender emailSender, SignInManager<AppUser> signInManager, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
	{
		_dbContext = dbContext;
		_userManager = userManager;
		_emailSender = emailSender;
		_signInManager = signInManager;
		_urlHelper = urlHelper;
		_httpContextAccessor = httpContextAccessor;
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
		//var res = await _userManager.FindByIdAsync(id);
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
