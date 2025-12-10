using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Register;
using Przetrwaj.Application.Dtos;
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
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterEmailCommand model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		try
		{
			var result = await _mediator.Send(model);
			return Ok(result);
		}
		catch (InvalidOperationException ex) //from handler
		{
			return BadRequest(ex.Message);
		}
		catch (NotImplementedException ex) //from email service
		{
			return BadRequest(ex.Message);
		}
	}
}
