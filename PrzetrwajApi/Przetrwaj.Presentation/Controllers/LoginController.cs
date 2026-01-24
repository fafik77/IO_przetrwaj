using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Commands.Login;
using Przetrwaj.Application.Settings;
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
	private readonly FrontEndSettings _frontEndSettings;

	public LoginController(IMediator mediator, IOptions<FrontEndSettings> options)
	{
		_mediator = mediator;
		_frontEndSettings = options.Value;
	}

	[HttpPost("email")]
	[SwaggerOperation("Login using email. 418 with info if user is banned")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status418ImATeapot)]
	public async Task<IActionResult> EmailLogin([FromBody] LoginEmailCommand model)
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

	[HttpGet("google")]
	[SwaggerOperation("Login using Google gmail")]
	public async Task<IActionResult> GoogleLogin()
	{
		var redirectUrl = Url.Action("GoogleResponse");
		if (redirectUrl is null) return NotFound("GoogleResponse endpoint not found");
		var command = new GoogleLoginCommand(redirectUrl);
		var properties = await _mediator.Send(command);
		return Challenge(properties, "Google");
	}

	[HttpGet("google-response")]
	[SwaggerOperation("Login using Google gmail. Redirects back to frontend")]
	[ProducesResponseType(StatusCodes.Status307TemporaryRedirect)]
	public async Task<IActionResult> GoogleResponse()
	{
		try
		{
			var result = await _mediator.Send(new GoogleLoginResponseCommand());
			return Redirect($"{_frontEndSettings.Url}/");
		}
		catch (UserBannedException ex)
		{
			return Redirect($"{_frontEndSettings.Url}/login-error/banned");
		}
		catch (BaseException ex)
		{
			return Redirect($"{_frontEndSettings.Url}/login-error/failed");
		}
	}
}
