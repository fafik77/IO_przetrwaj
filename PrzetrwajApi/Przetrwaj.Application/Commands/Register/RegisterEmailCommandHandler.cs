using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Register;

internal class RegisterEmailCommandHandler : ICommandHandler<RegisterEmailCommand, UserWithPersonalDataDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IAuthService _authService;

	public RegisterEmailCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IAuthService authService)
	{
		_unitOfWork = unitOfWork;
		_userRepository = userRepository;
		_authService = authService;
	}

	public async Task<UserWithPersonalDataDto> Handle(RegisterEmailCommand request, CancellationToken cancellationToken)
	{
		// The repository method now handles creating the AppUser, hashing the password, and saving it.
		// After this line, userToAdd is a tracked entity with a generated PasswordHash.
		var userToAdd = await _authService.RegisterUserByEmailAsync(request);
		var dto = (UserWithPersonalDataDto)userToAdd;
		return dto;
	}
}
