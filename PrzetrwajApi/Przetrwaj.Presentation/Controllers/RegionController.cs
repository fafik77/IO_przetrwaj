using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Regions;
using Przetrwaj.Application.Quaries.RegionQauries;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models.Dtos;
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
	[ProducesResponseType(typeof(IEnumerable<RegionOnlyDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var res = await _mediator.Send(new GetRegionsQuarry(), cancellationToken);
		return Ok(res);
	}

	[HttpGet("{id}")]
	[SwaggerOperation("Get Region")]
	[ProducesResponseType(typeof(RegionOnlyDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		try
		{
			var res = await _mediator.Send(new GetRegionQuarry() { IdRegion = id }, cancellationToken);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpPost]
	[SwaggerOperation("Add Region (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(typeof(RegionOnlyDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddRegion([FromBody] AddRegionCommand region, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var res = await _mediator.Send(region, cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = res.IdRegion }, res);
	}

	[HttpPut]
	[SwaggerOperation("Update Region (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateRegion([FromBody] UpdateRegionCommand region, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		try
		{
			await _mediator.Send(region, cancellationToken);
			return NoContent();
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpDelete("{id}")]
	[SwaggerOperation("Delete Region (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteRegion(int id, CancellationToken cancellationToken)
	{
		try
		{
			await _mediator.Send(new DeleteRegionCommand() { RegionId = id }, cancellationToken);
			return NoContent();
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}
}

