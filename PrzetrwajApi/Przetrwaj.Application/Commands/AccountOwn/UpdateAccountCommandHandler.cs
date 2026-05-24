using Microsoft.AspNetCore.Identity;
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
		bool userHasPassword = await _userManager.HasPasswordAsync(user);
		if (user is null) throw new UserNotFoundException(request.UserId);
		//if (request.Update.PowiatId != null) user.PowiatId = (short)request.Update.PowiatId;
		if (request.Update.Impediments != null) user.Impediments = (int)request.Update.Impediments;
		user.GminaId = request.Update.GminaId; //is nullable, so null it
		if (user.GminaId <= 0) user.GminaId = null; //null the invalid value
		if (!string.IsNullOrEmpty(request.Update.Name)) user.Name = request.Update.Name;
		if (!string.IsNullOrEmpty(request.Update.Surname)) user.Surname = request.Update.Surname;
		if (userHasPassword && !string.IsNullOrEmpty(request.Update.OldPassword))
		{
			//change password requires old password
			if (!string.IsNullOrEmpty(request.Update.NewPassword))
			{
				var identResult = await _userManager.ChangePasswordAsync(user, request.Update.OldPassword, request.Update.NewPassword);
				if (!identResult.Succeeded)
				{
					string errors = string.Join("\n", identResult.Errors
						.Where(e => e.Code.Contains("Password", StringComparison.OrdinalIgnoreCase))
						.Select(e => e.Description).ToList());
					if (string.IsNullOrEmpty(errors))
						throw new AccountUpdateException($"Could not update password: {request.Update.NewPassword}.\nTry another password");
					throw new AccountUpdateException(errors);
				}
			}
			//change email requires old password
			if (!string.IsNullOrEmpty(request.Update.Email))
			{
				var normName = _userManager.NormalizeName(request.Update.Email)!;
				var normOldName = user.NormalizedUserName;
				if (normName != normOldName)
				{
					//check if email is unique and password is correct
					var emailExists = await _userManager.FindByNameAsync(normName);
					if (emailExists != null) throw new UserAlreadyExistsException(request.Update.Email ?? request.UserId);
					var passCorrect = await _userManager.CheckPasswordAsync(user, request.Update.OldPassword);
					if (passCorrect == false) throw new AccountUpdateException("Incorrect Password");
					//now generate a token and send an email
					await _authService.GenerateChangeEmailTokenAsync(user, request.Update.Email);
				}
			}
		}
		else if (userHasPassword == false)
		{
			//set password
			if (!string.IsNullOrEmpty(request.Update.NewPassword))
			{
				var identResult = await _userManager.AddPasswordAsync(user, request.Update.NewPassword);
				if (!identResult.Succeeded)
				{
					string errors = string.Join("\n", identResult.Errors
						.Where(e => e.Code.Contains("Password", StringComparison.OrdinalIgnoreCase))
						.Select(e => e.Description).ToList());
					if (string.IsNullOrEmpty(errors))
						throw new AccountUpdateException($"Could not update password: {request.Update.NewPassword}.\nTry another password");
					throw new AccountUpdateException(errors);
				}
			}
			//change email is not available withouat a password
			if (!string.IsNullOrEmpty(request.Update.Email))
			{
				throw new AccountUpdateException("Password is not set, email change is not available!");
				//var normName = _userManager.NormalizeName(request.Email)!;
				//var normOldName = user.NormalizedUserName;
				//if (normName != normOldName)
				//{
				//	//check if email is unique
				//	var emailExists = await _userManager.FindByNameAsync(normName);
				//	if (emailExists != null) throw new UserAlreadyExistsException(request.Email ?? request.UserId);
				//	//now generate a token and send an email
				//	await _authService.GenerateChangeEmailTokenAsync(user, request.Email);
				//}
			}
		}

		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken); //this line can throw on email (yes when not unique)
		}
		catch (Exception)
		{
			throw new AccountUpdateException($"User: {request.Update.Email ?? request.UserId} already exists. Could not apply changes.");
		}
		return (UserWithPersonalDataDto)user;
	}
}
