using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Regions;
using Przetrwaj.Application.Quaries.Regions;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;


[Route("[controller]")]
[ApiController]
[Produces("application/json")]
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

	[HttpGet("woj")]
	[SwaggerOperation("Get Regions Województwa")]
	[ProducesResponseType(typeof(IEnumerable<RegionOnlyDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetWoj(CancellationToken cancellationToken)
	{
		var res = await _mediator.Send(new GetWojRegionsQuarry(), cancellationToken);
		return Ok(res);
	}

	[HttpGet("pow")]
	[SwaggerOperation("Get Regions Powiaty")]
	[ProducesResponseType(typeof(IEnumerable<RegionOnlyDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetPow(CancellationToken cancellationToken)
	{
		var res = await _mediator.Send(new GetPowRegionsQuarry(), cancellationToken);
		return Ok(res);
	}

	[HttpGet("gmi")]
	[SwaggerOperation("Get Regions Gminy")]
	[ProducesResponseType(typeof(IEnumerable<RegionOnlyDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetGmi(CancellationToken cancellationToken)
	{
		var res = await _mediator.Send(new GetGmiRegionsQuarry(), cancellationToken);
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

	//[HttpPost]
	//[SwaggerOperation("Add Region (Moderator)")]
	//[Authorize(UserRoles.Moderator)]
	//[ProducesResponseType(typeof(RegionOnlyDto), StatusCodes.Status201Created)]
	//[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	//public async Task<IActionResult> AddRegion([FromBody] AddRegionCommand region, CancellationToken cancellationToken)
	//{
	//	if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
	//	var res = await _mediator.Send(region, cancellationToken);
	//	return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
	//}

	[HttpPost("UpdateTercRegions")]
	[SwaggerOperation("Add or Update TERC Regions (Moderator)")]
	//[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(typeof(UpdateTercRegionsResults), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(UpdateTercRegionsResults), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[RequestFormLimits(MultipartBodyLengthLimit = 5 << 20)] //up to 5 MB
	[RequestSizeLimit(5 << 20)] //up to 5 MB
	public async Task<IActionResult> UpdateTercRegions([FromForm] UpdateTercRegionsCommand region, CancellationToken ct)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var res = await _mediator.Send(region, ct);
		return StatusCode((int)res.StatusCode, res);
	}

	[HttpPost("FromLocation")]
	[SwaggerOperation("Get region from location")]
	[ProducesResponseType(typeof(IRegionInfo), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> FromLocation([FromBody] LatLong region, CancellationToken ct)
	{
		return NoContent();
		//if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		//var res = await _mediator.Send(region, ct);
		//return StatusCode((int)res.StatusCode, res);
	}

	//[HttpPost("many")]
	//[SwaggerOperation("Add many Regions (Moderator)")]
	//[Authorize(UserRoles.Moderator)]
	//[ProducesResponseType(typeof(IEnumerable<RegionOnlyDto>), StatusCodes.Status201Created)]
	//[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	//public async Task<IActionResult> AddRegions([FromBody] AddRegionsCommand regions, CancellationToken cancellationToken)
	//{
	//	if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
	//	var res = await _mediator.Send(regions, cancellationToken);
	//	return CreatedAtAction(nameof(GetById), res);
	//}

	//[HttpPut]
	//[SwaggerOperation("Update Region (Moderator)")]
	//[Authorize(UserRoles.Moderator)]
	//[ProducesResponseType(StatusCodes.Status204NoContent)]
	//[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	//[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	//public async Task<IActionResult> UpdateRegion([FromBody] UpdateRegionCommand region, CancellationToken cancellationToken)
	//{
	//	if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
	//	try
	//	{
	//		await _mediator.Send(region, cancellationToken);
	//		return NoContent();
	//	}
	//	catch (BaseException ex)
	//	{
	//		return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
	//	}
	//}

	//[HttpDelete("{id}")]
	//[SwaggerOperation("Delete Region (Moderator)")]
	//[Authorize(UserRoles.Moderator)]
	//[ProducesResponseType(StatusCodes.Status204NoContent)]
	//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	//[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	//public async Task<IActionResult> DeleteRegion(int id, CancellationToken cancellationToken)
	//{
	//	try
	//	{
	//		await _mediator.Send(new DeleteRegionCommand() { RegionId = id }, cancellationToken);
	//		return NoContent();
	//	}
	//	catch (BaseException ex)
	//	{
	//		return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
	//	}
	//}
}

