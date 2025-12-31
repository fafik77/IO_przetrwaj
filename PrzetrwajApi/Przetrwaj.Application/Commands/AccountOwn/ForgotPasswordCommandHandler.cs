using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.AccountOwn;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, UserGeneralDto>
{
	private readonly IAuthService _authService;
	private readonly IUserRepository _userRepository;

	public ForgotPasswordCommandHandler(IAuthService authService, IUserRepository userRepository)
	{
		_authService = authService;
		_userRepository = userRepository;
	}

	public Task<UserGeneralDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
	{
		//_userManager.passw
		throw new NotImplementedException();
	}
}
