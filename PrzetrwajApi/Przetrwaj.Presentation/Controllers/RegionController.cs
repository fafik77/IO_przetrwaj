using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Regions;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Quaries.RegionQauries;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Exceptions._base;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

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
	[ProducesResponseType(typeof(IEnumerable<RegionOnlyDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll()
	{
		var res = await _mediator.Send(new GetRegionsQuarry());
		return Ok(res);
	}

	[HttpGet("{id}")]
	[SwaggerOperation("Get Region")]
	[ProducesResponseType(typeof(RegionOnlyDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(int id)
	{
		try
		{
			var res = await _mediator.Send(new GetRegionQuarry() { IdRegion = id });
			return Ok(res);
		}
		catch (BaseException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
		{
			return NotFound(ex.Message);
		}
	}

	[HttpPost]
	[SwaggerOperation("Add Region (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(typeof(RegionOnlyDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddRegion([FromBody] AddRegionCommand region)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		var res = await _mediator.Send(region);
		return CreatedAtAction(nameof(GetById), new { id = res.IdRegion }, res);
	}

	[HttpPut]
	[SwaggerOperation("Update Region (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateRegion([FromBody] UpdateRegionCommand region)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		try
		{
			await _mediator.Send(region);
			return NoContent();
		}
		catch (BaseException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
		{
			return NotFound(ex.Message);
		}
	}

	[HttpDelete("{id}")]
	[SwaggerOperation("Delete Region (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteRegion(int id)
	{
		try
		{
			await _mediator.Send(new DeleteRegionCommand() { RegionId = id });
			return NoContent();
		}
		catch (BaseException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
		{
			return NotFound(ex.Message);
		}
	}
}

