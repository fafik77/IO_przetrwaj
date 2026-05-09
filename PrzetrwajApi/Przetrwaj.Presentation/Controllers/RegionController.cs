using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Regions;
using Przetrwaj.Application.Quaries.Regions;
using Przetrwaj.Application.Queries.Regions;
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
		var res = await _mediator.Send(new GetGmiRegionsQuery(), cancellationToken);
		return Ok(res);
	}

	[HttpGet("{id}")]
	[SwaggerOperation("Get Region with TERC id")]
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

	[HttpPost("[action]")]
	[SwaggerOperation("Add or Update TERC Regions (.csv) (Moderator)")]
	[Authorize(UserRoles.Moderator)]
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

	[HttpPost("from-location")]
	[SwaggerOperation("Get region from location")]
	[ProducesResponseType(typeof(RegionOnlyDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> FromLocation([FromBody] LatLong region, CancellationToken ct)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		try
		{
			var res = await _mediator.Send(new RegionFromLocationQuery() { location = region }, ct);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

}

