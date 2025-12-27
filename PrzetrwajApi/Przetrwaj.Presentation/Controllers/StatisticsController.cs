using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Quaries.Stats;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class StatisticsController : Controller
{
	private readonly IMediator _mediator;

	public StatisticsController(IMediator mediator)
	{
		_mediator = mediator;
	}


	[HttpGet]
	[SwaggerOperation("Get statistics")]
	[ProducesResponseType(typeof(StatisticsDto), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
	{
		var res = await _mediator.Send(new GetStatisticsQuery(), cancellationToken);
		return Ok(res);
	}
}
