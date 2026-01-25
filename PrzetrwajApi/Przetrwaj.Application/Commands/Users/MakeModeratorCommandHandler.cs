using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Users
{
	public class MakeModeratorCommandHandler : ICommandHandler<MakeModeratorCommand, IdentityResult>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IUnitOfWork _unitOfWork;

		public MakeModeratorCommandHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
		{
			_userManager = userManager;
			_unitOfWork = unitOfWork;
		}

		public async Task<IdentityResult> Handle(MakeModeratorCommand request, CancellationToken cancellationToken)
		{
			AppUser? user;
			if (request.UserIdOrEmail.Contains('@')) //email
				user = await _userManager.FindByEmailAsync(request.UserIdOrEmail);
			else //id
				user = await _userManager.FindByIdAsync(request.UserIdOrEmail);
			if (user is null)
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });

			// Add the user to the Moderator role
			var result = await _userManager.AddToRoleAsync(user, UserRoles.Moderator);
			user.ModeratorRolePending = false;
			user.ModeratorSince = DateTime.UtcNow;
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
}
