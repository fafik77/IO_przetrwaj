using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Confirm;

public class ConfirmEmailChangeCommandHandler : ICommandHandler<ConfirmEmailChangeCommand, UserWithPersonalDataDto>
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public ConfirmEmailChangeCommandHandler(UserManager<AppUser> userManager, IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userManager = userManager;
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<UserWithPersonalDataDto> Handle(ConfirmEmailChangeCommand request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdAsync(request.UserId);
		if (user is null) throw new UserNotFoundException(request.UserId);
		var chEmRes = await _userManager.ChangeEmailAsync(user, request.NewEmail, request.Code);
		if (!chEmRes.Succeeded) throw new InvalidConfirmationException(string.Join("\n", chEmRes.Errors.Select(e => e.Description)));
		await _userManager.SetUserNameAsync(user, request.NewEmail);
		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Exception)
		{
			throw new InvalidConfirmationException("Could not update user account.");
		}
		return (UserWithPersonalDataDto)user;
	}
}
