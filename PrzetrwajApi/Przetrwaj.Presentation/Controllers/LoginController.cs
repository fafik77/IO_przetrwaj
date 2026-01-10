using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Login;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public class LoginController : Controller
{
	private readonly IMediator _mediator;

	public LoginController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("email")]
	[SwaggerOperation("Login using email. 418 with info if user is banned")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status418ImATeapot)]
	public async Task<IActionResult> LoginWithEmail([FromBody] LoginEmailCommand model)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		try
		{
			var result = await _mediator.Send(model);
			return Ok(result);
		}
		catch (UserBannedException ex)
		{
			return StatusCode(StatusCodes.Status418ImATeapot, ex.User);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}
}
