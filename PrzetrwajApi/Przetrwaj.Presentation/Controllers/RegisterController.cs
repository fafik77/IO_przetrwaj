using MediatR;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Register;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public partial class RegisterController : Controller
{
	private readonly IMediator _mediator;

	public RegisterController(IMediator mediator)
	{
		_mediator = mediator;
	}


	[HttpPost("email")]
	[SwaggerOperation("Register using email")]
	public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterEmailCommand model)
	{
		var result = await _mediator.Send(model);
		return Ok(result);
	}
}
