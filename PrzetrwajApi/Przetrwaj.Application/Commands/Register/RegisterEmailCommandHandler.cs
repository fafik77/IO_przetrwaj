using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Commands.Register;

internal class RegisterEmailCommandHandler : ICommandHandler<RegisterEmailCommand, RegisteredUserDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public RegisterEmailCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
	{
		_unitOfWork = unitOfWork;
		_userRepository = userRepository;
	}

	public async Task<RegisteredUserDto> Handle(RegisterEmailCommand request, CancellationToken cancellationToken)
	{
		// The repository method now handles creating the AppUser, hashing the password, and saving it.
		// After this line, userToAdd is a tracked entity with a generated PasswordHash.
		var userToAdd = await _userRepository.RegisterUserByEmailAsync(request.Email, request.Password, request.Name, request.Surname);
		var dto = (RegisteredUserDto)userToAdd;
		return dto;
	}
}
