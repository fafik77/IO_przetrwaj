using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Commands.Confirm;

public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, RegisteredUserDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public ConfirmEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<RegisteredUserDto> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
	{
		var res = await _userRepository.ConfirmEmailAsync(request.userId, request.code);
		return (RegisteredUserDto)res;
	}
}
