using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Impediments;
using Przetrwaj.Application.Quaries.Impediments;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;


[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public class ImpedimentController : Controller
{
	private readonly IMediator _mediator;

	public ImpedimentController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	[Authorize(UserRoles.Moderator)]
	[Consumes("application/json")]
	[SwaggerOperation("Create a Impediment Id:[0; 31] (Moderator)")]
	[ProducesResponseType(typeof(Impediment), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Create([FromBody] AddImpedimentCommand cmd, CancellationToken ct)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		try
		{
			var created = await _mediator.Send(cmd, ct);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpPut]
	[Authorize(UserRoles.Moderator)]
	[Consumes("application/json")]
	[SwaggerOperation("Update a Impediment Id:[0; 31] (Moderator)")]
	[ProducesResponseType(typeof(Impediment), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Update([FromBody] UpdateImpedimentCommand cmd, CancellationToken ct)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		try
		{
			var created = await _mediator.Send(cmd, ct);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpGet]
	[SwaggerOperation("List all Impediments")]
	[ProducesResponseType(typeof(IEnumerable<Impediment>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(CancellationToken ct)
	{
		var list = await _mediator.Send(new GetAllImpedimentsQuery(), ct);
		return Ok(list);
	}

	[HttpGet("{id}")]
	[SwaggerOperation("Get Impediment by id")]
	[ProducesResponseType(typeof(Impediment), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(short id, CancellationToken ct)
	{
		try
		{
			var res = await _mediator.Send(new GetImpedimentQuery() { Id = id }, ct);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}
}
