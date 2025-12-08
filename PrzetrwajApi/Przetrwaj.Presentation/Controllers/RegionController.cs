using MediatR;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.RegionCommands;
using Przetrwaj.Application.Quaries.Region;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;


[Route("[controller]")]
[ApiController]
public class RegionController : Controller
{
	private readonly IMediator _mediator;

	public RegionController(IMediator mediator)
	{
		_mediator = mediator;
	}


	[HttpGet]
	[SwaggerOperation("Get Regions")]
	public async Task<IActionResult> GetRegions()
	{
		var res = await _mediator.Send(new GetRegionsQuarry());
		return Ok(res);
	}

	[HttpGet("{id}")]
	[SwaggerOperation("Get Region")]
	public async Task<IActionResult> GetRegionById(int id)
	{
		var res = await _mediator.Send(new GetRegionQuarry() { IdRegion = id });
		return Ok(res);
	}

	[HttpPost]
	[SwaggerOperation("Add Region")]
	public async Task<IActionResult> AddRegion([FromBody] AddRegionCommand region)
	{
		var res = await _mediator.Send(region);
		return Ok(res);
	}

	[HttpPut]
	[SwaggerOperation("Update Region")]
	public async Task<IActionResult> UpdateRegion([FromBody] UpdateRegionCommand region)
	{
		await _mediator.Send(region);
		return NoContent();
	}

	[HttpDelete("{id}")]
	[SwaggerOperation("Delete Region")]
	public async Task<IActionResult> DeleteRegion(int id)
	{
		throw new NotImplementedException();
	}
}

