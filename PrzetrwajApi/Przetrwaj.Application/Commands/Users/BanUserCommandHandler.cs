using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Users;

public class BanUserCommandHandler : ICommandHandler<BanUserInternallCommand, UserWithPersonalDataDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly UserManager<AppUser> _userManager;
	private readonly IBanCache _banCache;

	public BanUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IBanCache banCache)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_userManager = userManager;
		_banCache = banCache;
	}

	public async Task<UserWithPersonalDataDto> Handle(BanUserInternallCommand request, CancellationToken cancellationToken)
	{
		AppUser? user;
		// 1. Get the ID of the currently logged-in user
		// The ClaimTypes.NameIdentifier holds the user's Id from the Identity system.
		if (request.UserIdOrEmail.Contains('@')) //email
			user = await _userRepository.GetByEmailAsync(request.UserIdOrEmail, cancellationToken);
		else //id
			user = await _userRepository.GetByIdAsync(request.UserIdOrEmail, cancellationToken);
		if (user is null) throw new UserNotFoundException(request.UserIdOrEmail);
		if (user.Banned || user.BannedById != null) //user was already banned
		{
			AppUser? moderatorOld = await _userRepository.GetByIdAsync(user.BannedById!, cancellationToken);
			var dtoUserAlreadyBanned = (UserWithPersonalDataDto)user;
			if (dtoUserAlreadyBanned.BanInfo != null)
				dtoUserAlreadyBanned.BanInfo.BannedBy = (UserGeneralDto?)moderatorOld; //add Moderator info
			return dtoUserAlreadyBanned;
		}
		AppUser? moderator = await _userRepository.GetByIdAsync(request.ModeratorId, cancellationToken);
		if (moderator is null) throw new UserNotFoundException(request.ModeratorId);

		bool moderatorIsAdmin = await _userManager.IsInRoleAsync(moderator, UserRoles.Admin);
		var userRoles = await _userManager.GetRolesAsync(user);
		if (userRoles.Contains(UserRoles.Admin) || (moderatorIsAdmin && userRoles.Contains(UserRoles.Moderator)))
		{   //Admin can not be banned. Moderator can only be banned by Admin (authorization levels).
			var rolesStr = string.Join(", ", userRoles);
			throw new UnauthorizedException($"User {request.UserIdOrEmail} has roles: {rolesStr}. You {moderator.Name} {moderator.Surname} do not have permissions to ban them.");
		}

		user.BanDate = DateTimeOffset.UtcNow;
		user.BanReason = request.Reason;
		user.BannedById = request.ModeratorId;
		user.Banned = true;
		// This is the key line to invalidate logged in user cookie or other tokens:
		await _userManager.UpdateSecurityStampAsync(user);
		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
		// cache ban info
		_banCache.BanUser(user.Id);

		var dto = (UserWithPersonalDataDto)user;
		if (dto.BanInfo != null)
			dto.BanInfo.BannedBy = (UserGeneralDto?)moderator; //add Moderator info
		return dto;
	}
}
