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
	private readonly IRegionRepository _regionRepository;

	public UpdateAccountCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IAuthService authService, IRegionRepository regionRepository)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_userManager = userManager;
		_authService = authService;
		_regionRepository = regionRepository;
	}


	public async Task<UserWithPersonalDataDto> Handle(UpdateAccountInternallCommand request, CancellationToken ct)
	{
		request.UserId = request.UserId.ToLower();
		var user = await _userRepository.GetByIdAsync(request.UserId, ct);
		if (user is null) throw new UserNotFoundException(request.UserId);
		var email = user.Email;

		if (request.Update is UpdateAccountCommand updateInfo)
		{
			await UpdateGeneralInfo(user, updateInfo, ct);
		}
		else if (request.Update is UpdateAccountEmailCommand updateEmail)
		{
			email = updateEmail.Email;
			await UpdateEmail(user, updateEmail);
		}
		else if (request.Update is UpdateAccountPasswordCommand updatePassword)
		{
			await UpdatePassword(user, updatePassword);
		}

		try
		{
			await _unitOfWork.SaveChangesAsync(ct); //this line can throw on email (when not unique), it's checked but concurency is a thing
		}
		catch (Exception)
		{
			throw new AccountUpdateException($"User: {email} already exists. Could not apply changes.");
		}
		return (UserWithPersonalDataDto)user;
	}

	private async Task UpdatePassword(AppUser user, UpdateAccountPasswordCommand updatePassword)
	{
		bool userHasPassword = await _userManager.HasPasswordAsync(user);
		IdentityResult identResult;

		//change password requires old password
		if (userHasPassword)
		{
			if (updatePassword.OldPassword is null)
				throw new AccountPasswordUpdateException("Invalid Password");
			identResult = await _userManager.ChangePasswordAsync(user, updatePassword.OldPassword, updatePassword.NewPassword);
		}
		//set password
		else
			identResult = await _userManager.AddPasswordAsync(user, updatePassword.NewPassword);

		if (!identResult.Succeeded)
		{
			string errors = string.Join("\n", identResult.Errors
				.Where(e => e.Code.Contains("Password", StringComparison.OrdinalIgnoreCase))
				.Select(e => e.Description).ToList());
			if (string.IsNullOrEmpty(errors))
				throw new AccountPasswordUpdateException($"Could not update password: {updatePassword.NewPassword}.\nTry another password");
			throw new AccountPasswordUpdateException(errors);
		}
	}

	private async Task UpdateEmail(AppUser user, UpdateAccountEmailCommand updateEmail)
	{
		bool userHasPassword = await _userManager.HasPasswordAsync(user);
		//change email is not available withouat a password
		if (userHasPassword == false)
			throw new AccountEmailUpdateException("Password is required to change email!");

		//change email requires old password
		var normName = _userManager.NormalizeName(updateEmail.Email)!;
		var normOldName = user.NormalizedUserName;
		if (normName == normOldName)
			return;

		//check if email is unique and password is correct
		if (await _userManager.CheckPasswordAsync(user, updateEmail.OldPassword) == false)
			throw new AccountEmailUpdateException("Incorrect Password");
		if (await _userManager.FindByNameAsync(normName) != null)
			throw new UserAlreadyExistsException(updateEmail.Email ?? user.Id);
		//now generate a token and send an email
		await _authService.GenerateChangeEmailTokenAsync(user, updateEmail.Email, updateEmail.ReturnUrl);
	}

	private async Task UpdateGeneralInfo(AppUser user, UpdateAccountCommand update, CancellationToken ct)
	{
		if (update.Impediments != null) user.Impediments = (int)update.Impediments;
		if (update.GminaId != null)
		{
			var gmi = await _regionRepository.GetByIdAsync(update.GminaId.Value, ct);
			if (gmi != null && gmi.Type == RegionPrecision.GMI)
				user.GminaId = update.GminaId;
		}
		if (!string.IsNullOrEmpty(update.Name)) user.Name = update.Name;
		if (!string.IsNullOrEmpty(update.Surname)) user.Surname = update.Surname;
	}
}
