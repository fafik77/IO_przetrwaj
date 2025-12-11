using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Categories;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Quaries.Categories;
using Przetrwaj.Domain;

namespace Przetrwaj.Presentation.Controllers;

[ApiController]
[Route("Category/Resource")]
public class ResourceCategoriesController : ControllerBase
{
	private readonly IMediator _mediator;
	public ResourceCategoriesController(IMediator mediator) => _mediator = mediator;

	[HttpPost]
	[Authorize(UserRoles.Moderator)]
	public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateResourceCategoryCommand cmd, CancellationToken ct)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		var created = await _mediator.Send(cmd, ct);
		return CreatedAtAction(nameof(GetById), new { id = created.IdCategory }, created);
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
	{
		var list = await _mediator.Send(new GetResourcesCategoriesQuery(), ct);
		return Ok(list);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
	{
		var item = await _mediator.Send(new GetResourceCategoryByIdQuery { IdCategory = id }, ct);
		return item is null ? NotFound() : Ok(item);
	}

	[HttpDelete("{id:int}")]
	[Authorize(UserRoles.Moderator)]
	public async Task<IActionResult> Delete(int id, CancellationToken ct)
	{
		var ok = await _mediator.Send(new DeleteResourceCategoryCommand { IdCategory = id }, ct);
		return ok ? NoContent() : NotFound();
	}
}
