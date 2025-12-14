using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class StatsController : Controller
{
	private readonly IMediator _mediator;

	public StatsController(IMediator mediator)
	{
		_mediator = mediator;
	}


	[HttpGet]
	[SwaggerOperation("Get statistics")]
	[ProducesResponseType(typeof(StatisticsDto), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetStatistics()
	{
		throw new NotImplementedException();
	}
}
