using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Users;

namespace Przetrwaj.Application.Commands.Users;

public class DenyModeratorCommandHandler : ICommandHandler<DenyModeratorCommand, AppUser>
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IUnitOfWork _unitOfWork;

	public DenyModeratorCommandHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
	{
		_userManager = userManager;
		_unitOfWork = unitOfWork;
	}

	public async Task<AppUser> Handle(DenyModeratorCommand request, CancellationToken cancellationToken)
	{
		AppUser? user;
		if (request.UserIdOrEmail.Contains('@')) //email
			user = await _userManager.FindByEmailAsync(request.UserIdOrEmail);
		else //id
			user = await _userManager.FindByIdAsync(request.UserIdOrEmail);
		if (user is null)
			throw new UserNotFoundException(request.UserIdOrEmail);

		// Remove the Moderator role flag
		user.ModeratorRolePending = false;
		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
		return user;
	}
}
