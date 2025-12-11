using MediatR;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Commands.Categories;
using Przetrwaj.Application.Quaries.Categories;


namespace Przetrwaj.Presentation.Controllers;

[ApiController]
[Route("Category/Danger")]
public class DangerCategoriesController : Controller
{
    private readonly IMediator _mediator;
    public DangerCategoriesController(IMediator mediator) => _mediator = mediator;

    // POST /categories/danger
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateDangerCategoryCommand cmd, CancellationToken ct)
    {
        var created = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.IdCategory }, created);
    }

    // GET /categories/danger
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var list = await _mediator.Send(new GetDangerCategoriesQuery(), ct);
        return Ok(list);
    }

    // GET /categories/danger/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
    {
        var item = await _mediator.Send(new GetDangerCategoryByIdQuery { IdCategory = id }, ct);
        return item is null ? NotFound() : Ok(item);
    }

    // DELETE /categories/danger/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteDangerCategoryCommand { IdCategory = id }, ct);
        return ok ? NoContent() : NotFound();
    }
}
