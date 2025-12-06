using Microsoft.AspNet.Identity.EntityFramework;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

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
		var res = new RegisteredUserDto();
		return res;
		//throw new NotImplementedException();
		//if (!request.IsValid || string.IsNullOrEmpty(request.Password))
		//	throw new ValidationException("Invalid registration details.");

		//var user = new IdentityUser { UserName = model.Email, Email = model.Email };
		//var result = await _userManager.CreateAsync(user, model.Password);

		//if (!result.Succeeded)
		//	return BadRequest(result.Errors);

		//return Ok("User registered successfully.");
	}
}
