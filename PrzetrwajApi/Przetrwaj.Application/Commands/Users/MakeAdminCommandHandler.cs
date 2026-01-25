using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Users;

public class MakeAdminCommandHandler : ICommandHandler<MakeAdminInternallCommand, IdentityResult>
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IUnitOfWork _unitOfWork;

	public MakeAdminCommandHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
	{
		_userManager = userManager;
		_unitOfWork = unitOfWork;
	}

	public async Task<IdentityResult> Handle(MakeAdminInternallCommand request, CancellationToken cancellationToken)
	{
		//authenticate Admin requester
		var admin = await _userManager.FindByIdAsync(request.Id);
		if (admin is null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
		var passCorrect = await _userManager.CheckPasswordAsync(admin, request.Password);
		if (passCorrect == false) return IdentityResult.Failed(new IdentityError { Description = "Incorrect password." });

		//Find and grand Admin role to user
		AppUser? user;
		if (request.UserIdOrEmail.Contains('@')) //email
			user = await _userManager.FindByEmailAsync(request.UserIdOrEmail);
		else //id
			user = await _userManager.FindByIdAsync(request.UserIdOrEmail);
		if (user is null)
			return IdentityResult.Failed(new IdentityError { Description = "User not found." });

		// Add the user to the Admin role
		var result = await _userManager.AddToRoleAsync(user, UserRoles.Admin);
		user.ModeratorRolePending = false;
		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
		return result;
	}
}
