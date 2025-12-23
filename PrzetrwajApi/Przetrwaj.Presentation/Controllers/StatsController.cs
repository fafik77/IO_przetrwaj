using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("WIP/[controller]")]
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
	public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
