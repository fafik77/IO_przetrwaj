using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Users
{
	public class MakeModeratorCommandHandler : ICommandHandler<MakeModeratorCommand, IdentityResult>
	{
		private readonly UserManager<AppUser> _userManager;

		public MakeModeratorCommandHandler(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IdentityResult> Handle(MakeModeratorCommand request, CancellationToken cancellationToken)
		{
			AppUser? user;
			if (request.UserIdOrEmail.Contains("@")) //email
				user = await _userManager.FindByEmailAsync(request.UserIdOrEmail);
			else //id
				user = await _userManager.FindByIdAsync(request.UserIdOrEmail);
			if (user is null)
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });

			// Add the user to the Moderator role
			var result = await _userManager.AddToRoleAsync(user, UserRoles.Moderator);
			return result;
		}
	}
}
