using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przetrwaj.Application.Commands.Login
{
	public class LoginEmailCommandHandler : ICommandHandler<LoginEmailCommand, RegisteredUserDto>
	{
		private readonly IUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;

		public LoginEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
		{
			_userRepository = userRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<RegisteredUserDto> Handle(LoginEmailCommand request, CancellationToken cancellationToken)
		{
			var registeredUser = await _userRepository.LoginUserByEmailAsync(request.Email, request.Password);
			if (registeredUser == null) throw new InvalidOperationException("Could not Login");
			var dto = (RegisteredUserDto) registeredUser;
			return dto;
		}
	}
}
