using Microsoft.AspNetCore.Identity;
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

	public async Task<AppUser> RegisterUserByEmailAsync(string email, string password, string name, string surname)
	{
		var user = new AppUser
		{
			Email = email,
			Name = name,
			Surname = surname,
			UserName = email, // Typically, UserName is set to the email for login (its enforced unique)
			IdRegion = 0,
		};

		var result = await _userManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			// Handle errors, e.g., throw an exception with IdentityResult.Errors
			throw new InvalidOperationException("User creation failed.");
		}
		return user;
	}
}
