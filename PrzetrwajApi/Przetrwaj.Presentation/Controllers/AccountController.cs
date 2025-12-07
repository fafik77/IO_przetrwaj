using MediatR;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Confirm;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class AccountController : Controller
{
	private readonly IMediator _mediator;

	public AccountController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("ConfirmEmail")]
	[SwaggerOperation("Confirm Email")]
	public async Task<IActionResult> ConfirmEmail(string userId, string code)
	{
		if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
			return BadRequest("Invalid email confirmation request.");

		var command = new ConfirmEmailCommand { userId = userId, code = code };
		var res = await _mediator.Send(command);
		if (res == null) return BadRequest("Invalid email confirmation request.");
		return Ok(res);
	}
}
