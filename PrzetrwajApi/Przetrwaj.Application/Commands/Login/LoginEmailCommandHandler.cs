using Microsoft.AspNetCore.Identity;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Login;

public class LoginEmailCommandHandler : ICommandHandler<LoginEmailCommand, JwtTokenDto>
{
	private readonly IAuthService _authService;
	private readonly UserManager<AppUser> _userManager;
	private readonly IJwtService _jwtService;

	public LoginEmailCommandHandler(IAuthService authService, UserManager<AppUser> userManager, IJwtService jwtService)
	{
		_authService = authService;
		_userManager = userManager;
		_jwtService = jwtService;
	}

	public async Task<JwtTokenDto> Handle(LoginEmailCommand request, CancellationToken cancellationToken)
	{
		var registeredUser = await _authService.LoginUserByEmailAsync(request.Email, request.Password);
		if (registeredUser == null) throw new InvalidLoginException("Could not Login");
		var dto = (UserWithPersonalDataDto)registeredUser;
		var roles = await _userManager.GetRolesAsync(registeredUser);
		dto.Roles = roles;
		return await _jwtService.GenerateTokenAsync(dto);
	}
}
