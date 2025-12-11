using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Categories;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Quaries.Categories;
using Przetrwaj.Domain;
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
    [SwaggerOperation("Create a Danger category (Moderator only)")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateDangerCategoryCommand cmd, CancellationToken ct)
    {
 
        if (cmd is null) return BadRequest("Body is required.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrWhiteSpace(cmd.Name)) return BadRequest("Name is required.");
        if (cmd.Name.Length < 3 || cmd.Name.Length > 400) return BadRequest("Name must be 3–400 characters.");

        var created = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.IdCategory }, created);
    }


    [HttpGet]
    [SwaggerOperation("List all Danger categories")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var list = await _mediator.Send(new GetDangerCategoriesQuery(), ct);
        return Ok(list);
    }

  
    [HttpGet("{id:int}")]
    [SwaggerOperation("Get Danger category by id")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
    {

        if (id <= 0) return BadRequest("Id must be a positive integer.");

        var item = await _mediator.Send(new GetDangerCategoryByIdQuery { IdCategory = id }, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:int}")]
    [Authorize(UserRoles.Moderator)]
    [SwaggerOperation("Delete Danger category by id (Moderator only)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (id <= 0) return BadRequest("Id must be a positive integer.");

        var ok = await _mediator.Send(new DeleteDangerCategoryCommand { IdCategory = id }, ct);
        return ok ? NoContent() : NotFound();
    }
}
