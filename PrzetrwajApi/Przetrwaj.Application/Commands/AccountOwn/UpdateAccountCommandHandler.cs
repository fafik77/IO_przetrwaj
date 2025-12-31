using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.AuthServices;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class UpdateAccountCommandHandler : ICommandHandler<UpdateAccountInternallCommand, UserWithPersonalDataDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IAuthService _authService;
	private readonly UserManager<AppUser> _userManager;

	public UpdateAccountCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IAuthService authService)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_userManager = userManager;
		_authService = authService;
	}


	public async Task<UserWithPersonalDataDto> Handle(UpdateAccountInternallCommand request, CancellationToken cancellationToken)
	{
		string ChangeEmailToken = string.Empty;
		request.UserId = request.UserId.ToLower();
		var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
		if (user is null) throw new UserNotFoundException(request.UserId);
		if (request.IdRegion != null) user.IdRegion = (int)request.IdRegion;
		if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
		if (!string.IsNullOrEmpty(request.Surname)) user.Surname = request.Surname;
		if (!string.IsNullOrEmpty(request.OldPassword))
		{
			//change password requires old password
			if (!string.IsNullOrEmpty(request.NewPassword))
			{
				var identResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
				if (!identResult.Succeeded)
				{
					string errors = string.Join("\n", identResult.Errors
						.Where(e => e.Code.Contains("Password", StringComparison.OrdinalIgnoreCase))
						.Select(e => e.Description).ToList());
					if (string.IsNullOrEmpty(errors))
						throw new AccountUpdateException($"Could not update password: {request.NewPassword}.\nTry another password");
					throw new AccountUpdateException(errors);
				}
			}
			//change email requires old password
			if (!string.IsNullOrEmpty(request.Email))
			{
				var normName = _userManager.NormalizeName(request.Email)!;
				var normOldName = user.NormalizedUserName;
				if (normName != normOldName)
				{
					//check if email is unique and password is correct
					var emailExists = await _userManager.FindByNameAsync(normName);
					if (emailExists != null) throw new UserAlreadyExistsException(request.Email ?? request.UserId);
					var passCorrect = await _userManager.CheckPasswordAsync(user, request.OldPassword);
					if (passCorrect == false) throw new AccountUpdateException("Incorrect Password");
					//now generate a token and send an email
					await _authService.GenerateChangeEmailTokenAsync(user, request.Email);
				}
			}
		}
		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken); //this line can throw on email (yes when not unique)
		}
		catch (Exception)
		{
			throw new AccountUpdateException($"User: {request.Email ?? request.UserId} already exists. Could not apply changes.");
		}
		return (UserWithPersonalDataDto)user;
	}
}
