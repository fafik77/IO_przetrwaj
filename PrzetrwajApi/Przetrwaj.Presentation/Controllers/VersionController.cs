using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Queries.Version;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public class VersionController : Controller
{
	private readonly IMediator _mediator;

	public VersionController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet]
	[AllowAnonymous]
	[SwaggerOperation("Get application build version")]
	[ProducesResponseType(typeof(AppVersionDto), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetVersion(CancellationToken cancellationToken)
	{
		var res = await _mediator.Send(new GetAppVersionQuery(), cancellationToken);
		return Ok(res);
	}
}
