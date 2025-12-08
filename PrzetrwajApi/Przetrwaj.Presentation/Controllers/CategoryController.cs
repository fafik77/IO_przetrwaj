using MediatR;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Categories;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Quaries.Categories;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoryController : Controller
{
	private readonly IMediator _mediator;

	public CategoryController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	[SwaggerOperation("Create category")]
	public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
	{
		//if (!ModelState.IsValid)
		//	return BadRequest(ModelState);

		var result = await _mediator.Send(command);
		return CreatedAtAction(nameof(GetById), new { id = result.IdCategory }, result);
	}

	[HttpGet]
	[SwaggerOperation("Get all categories")]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
	{
		var result = await _mediator.Send(new GetCategoriesQuery());
		return Ok(result);
	}

	[HttpGet("{id:int}")]
	[SwaggerOperation("Get category by id")]
	public async Task<ActionResult<CategoryDto>> GetById(int id)
	{
		var result = await _mediator.Send(new GetCategoryByIdQuery { IdCategory = id });
		if (result is null)
			return NotFound();

		return Ok(result);
	}
}

