using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Users;

namespace Przetrwaj.Application.Commands.Users;

public class BanUserCommandHandler : ICommandHandler<BanUserInternallCommand, UserWithPersonalDataDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public BanUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}


	public async Task<UserWithPersonalDataDto> Handle(BanUserInternallCommand request, CancellationToken cancellationToken)
	{
		//bool isSelf = false;
		AppUser user;
		// 1. Get the ID of the currently logged-in user
		// The ClaimTypes.NameIdentifier holds the user's Id from the Identity system.
		if (request.UserIdOrEmail.Contains("@")) //email
			user = await _userRepository.GetByEmailAsync(request.UserIdOrEmail);
		else //id
			user = await _userRepository.GetByIdAsync(request.UserIdOrEmail);
		if (user == null) throw new UserNotFoundException(request.UserIdOrEmail);
		if (user.Banned || user.BannedById != null) //user was already banned
		{
			AppUser moderatorOld = await _userRepository.GetByIdAsync(user.BannedById);
			var dto2 = (UserWithPersonalDataDto)user;
			dto2.BannedBy = (UserGeneralDto?)moderatorOld; //add Moderator info
			return dto2;
		}

		AppUser moderator = await _userRepository.GetByIdAsync(request.ModeratorId);

		user.BanDate = DateTimeOffset.UtcNow;
		user.BanReason = request.Reason;
		user.BannedById = request.ModeratorId;
		user.Banned = true;

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		var dto = (UserWithPersonalDataDto)user;
		dto.BannedBy = (UserGeneralDto?)moderator; //add Moderator info
		return dto;
	}
}
