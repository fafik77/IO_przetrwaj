using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Categories;
using Przetrwaj.Application.Quaries.Categories;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Categories;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[ApiController]
[Route("Category/Danger")]

[Produces("application/json")]
public class DangerCategoriesController : Controller
{
	private readonly IMediator _mediator;
	public DangerCategoriesController(IMediator mediator) => _mediator = mediator;

	[HttpPost]
	[Authorize(UserRoles.Moderator)]
	[Consumes("application/json")]
	[SwaggerOperation("Create a Danger category (Moderator)")]
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Create([FromBody] CreateDangerCategoryCommand cmd, CancellationToken ct)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var created = await _mediator.Send(cmd, ct);
		return CreatedAtAction(nameof(GetById), new { id = created.IdCategory }, created);
	}

	[HttpPost("many")]
	[Authorize(UserRoles.Moderator)]
	[Consumes("application/json")]
	[SwaggerOperation("Create many Danger categories (Moderator)")]
	[ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Create([FromBody] CreateDangerCategoriesCommand cmd, CancellationToken ct)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var created = await _mediator.Send(cmd, ct);
		return CreatedAtAction(nameof(GetById), created);
	}


	[HttpGet]
	[SwaggerOperation("List all Danger categories")]
	[ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(CancellationToken ct)
	{
		var list = await _mediator.Send(new GetDangerCategoriesQuery(), ct);
		return Ok(list);
	}


	[HttpGet("{id:int}")]
	[SwaggerOperation("Get Danger category by id")]
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(int id, CancellationToken ct)
	{
		var item = await _mediator.Send(new GetDangerCategoryByIdQuery { IdCategory = id }, ct);
		return item != null ? Ok(item) : NotFound((ExceptionCasting)new CategoryNotFoundException(id));
	}

	[HttpDelete("{id:int}")]
	[Authorize(UserRoles.Moderator)]
	[SwaggerOperation("Delete Danger category by id (Moderator)")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Delete(int id, CancellationToken ct)
	{
		var ok = await _mediator.Send(new DeleteDangerCategoryCommand { IdCategory = id }, ct);
		return ok ? NoContent() : NotFound((ExceptionCasting)new CategoryNotFoundException(id));
	}
}
