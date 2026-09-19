using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Quaries.Categories;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[ApiController]
[Route("Categories")]
[Produces("application/json")]
public class CategoriesController(IMediator mediator) : Controller
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [SwaggerOperation("List all categories (order by ID)")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var list = await _mediator.Send(new GetAllCategoriesQuery(), ct);
        return Ok(list);
    }
}
