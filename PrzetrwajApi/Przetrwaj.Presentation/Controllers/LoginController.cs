using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Login;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : Controller
{
	private const string AuthenticationCookie = "cookie";
	private readonly IMediator _mediator;

	public LoginController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("email")]
	[SwaggerOperation("Login using email. BadRequest with info if user is banned")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> LoginWithEmail([FromBody] LoginEmailCommand model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		try
		{
			var result = await _mediator.Send(model);
			if (result == null) return BadRequest("Invalid Credentials");
			if (result.Banned) return BadRequest(result);

			return Ok(result);
		}
		catch (InvalidLoginException invalidLogin)
		{
			return BadRequest(invalidLogin.Message);
		}
		catch (Exception)
		{
			throw;
		}
	}
}
