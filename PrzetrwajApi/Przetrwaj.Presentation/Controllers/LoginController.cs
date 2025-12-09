using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Login;
using Przetrwaj.Application.Dtos;
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
	[SwaggerOperation("Login using email. BadRequest if user is banned")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> LoginWithEmail([FromBody] LoginEmailCommand model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _mediator.Send(model);
		if (result == null) return BadRequest("Invalid Credentials");
		if (result.Banned) return BadRequest(result);

		return Created("", result);
	}
}
