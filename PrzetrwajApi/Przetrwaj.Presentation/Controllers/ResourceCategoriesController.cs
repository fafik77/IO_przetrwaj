using MediatR;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Commands.Categories;
using Przetrwaj.Application.Quaries.Categories;

namespace Przetrwaj.Presentation.Controllers;

[ApiController]
[Route("categories/resources")]
public class ResourceCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ResourceCategoriesController(IMediator mediator) => _mediator = mediator;

    // POST /categories/resources
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateResourceCategoryCommand cmd, CancellationToken ct)
    {
        var created = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.IdCategory }, created);
    }

    // GET /categories/resources
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var list = await _mediator.Send(new GetResourcesCategoriesQuery(), ct);
        return Ok(list);
    }

    // GET /categories/resources/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
    {
        var item = await _mediator.Send(new GetResourceCategoryByIdQuery { IdCategory = id }, ct);
        return item is null ? NotFound() : Ok(item);
    }

    // DELETE /categories/resources/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteResourceCategoryCommand { IdCategory = id }, ct);
        return ok ? NoContent() : NotFound();
    }
}
