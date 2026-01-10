using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Register;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
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
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterEmailCommand model)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		try
		{
			var result = await _mediator.Send(model);
			return Ok(result);
		}
		catch (BaseException ex)
		{
			return BadRequest((ExceptionCasting)ex);
		}
		catch (NotImplementedException ex)
		{
			return BadRequest((ExceptionCasting)new RegisterException(ex.Message));
		}
	}
}
