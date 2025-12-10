using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Login
{
	public class LoginEmailCommandHandler : ICommandHandler<LoginEmailCommand, UserWithPersonalDataDto>
	{
		private readonly IUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuthService _authService;

		public LoginEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthService authService)
		{
			_userRepository = userRepository;
			_unitOfWork = unitOfWork;
			_authService = authService;
		}

		public async Task<UserWithPersonalDataDto> Handle(LoginEmailCommand request, CancellationToken cancellationToken)
		{
			var registeredUser = await _authService.LoginUserByEmailAsync(request.Email, request.Password);
			if (registeredUser == null) throw new InvalidLoginException("Could not Login");
			var dto = (UserWithPersonalDataDto)registeredUser;
			return dto;
		}
	}
}
