using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Login
{
	public class LoginEmailCommandHandler : ICommandHandler<LoginEmailCommand, UserWithPersonalDataDto>
	{
		private readonly IUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuthService _authService;
		private readonly UserManager<AppUser> _userManager;

		public LoginEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthService authService, UserManager<AppUser> userManager)
		{
			_userRepository = userRepository;
			_unitOfWork = unitOfWork;
			_authService = authService;
			_userManager = userManager;
		}

		public async Task<UserWithPersonalDataDto> Handle(LoginEmailCommand request, CancellationToken cancellationToken)
		{
			var registeredUser = await _authService.LoginUserByEmailAsync(request.Email, request.Password);
			if (registeredUser == null) throw new InvalidLoginException("Could not Login");
			var dto = (UserWithPersonalDataDto)registeredUser;
			var roles = await _userManager.GetRolesAsync(registeredUser);
			dto.Role = string.Join(", ", roles);
			return dto;
		}
	}
}
