using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Commands.Confirm;

public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, UserWithPersonalDataDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IAuthService _authService;

	public ConfirmEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthService authService)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_authService = authService;
	}

	public async Task<UserWithPersonalDataDto> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
	{
		var res = await _authService.ConfirmEmailAsync(request.userId, request.code);
		return (UserWithPersonalDataDto)res;
	}
}
