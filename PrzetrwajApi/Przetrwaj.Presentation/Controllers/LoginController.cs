using MediatR;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Login;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : Controller
{
	private readonly IMediator _mediator;

	public LoginController(IMediator mediator)
	{
		_mediator = mediator;
	}


	[HttpPost("email")]
	[SwaggerOperation("Login using email")]
	public async Task<IActionResult> LoginWithEmail([FromBody] LoginEmailCommand model)
	{
		var result = await _mediator.Send(model);
		return Created("", result);
	}
}
