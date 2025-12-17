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
[Route("Category/Resource")]
[Produces("application/json")]
public class ResourceCategoriesController : ControllerBase
{
	private readonly IMediator _mediator;
	public ResourceCategoriesController(IMediator mediator) => _mediator = mediator;


	[HttpPost]
	[Authorize(UserRoles.Moderator)]
	[Consumes("application/json")]
	[SwaggerOperation("Create a Resource category (Moderator only)")]
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateResourceCategoryCommand cmd, CancellationToken ct)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var created = await _mediator.Send(cmd, ct);
		return CreatedAtAction(nameof(GetById), new { id = created.IdCategory }, created);
	}


	[HttpGet]
	[SwaggerOperation("List all Resource categories")]
	[ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
	{
		var list = await _mediator.Send(new GetResourcesCategoriesQuery(), ct);
		return Ok(list);
	}


	[HttpGet("{id:int}")]
	[SwaggerOperation("Get Resource category by id")]
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
	{
		var item = await _mediator.Send(new GetResourceCategoryByIdQuery { IdCategory = id }, ct);
		return item != null ? Ok(item) : NotFound((ExceptionCasting)new CategoryNotFoundException(id));
	}

	[HttpDelete("{id:int}")]
	[Authorize(UserRoles.Moderator)]
	[SwaggerOperation("Delete Resource category by id (Moderator only)")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(int id, CancellationToken ct)
	{
		var ok = await _mediator.Send(new DeleteResourceCategoryCommand { IdCategory = id }, ct);
		return ok ? NoContent() : NotFound((ExceptionCasting)new CategoryNotFoundException(id));
	}
}
