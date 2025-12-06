using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Przetrwaj.Application.Commands.Register;

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
		return Created("", result);
		//if (!ModelState.IsValid || string.IsNullOrEmpty(model.Password))
		//	return BadRequest("Invalid registration details.");

		//var user = new IdentityUser { UserName = model.Email, Email = model.Email };
		//var result = await _userManager.CreateAsync(user, model.Password);

		//if (!result.Succeeded)
		//	return BadRequest(result.Errors);

		//return Ok("User registered successfully.");
	}
}
