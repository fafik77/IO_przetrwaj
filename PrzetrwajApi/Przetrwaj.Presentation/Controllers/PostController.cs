using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("WIP/[controller]")]
[ApiController]
public class PostController : Controller
{
	private readonly IMediator _mediator;

	public PostController(IMediator mediator)
	{
		_mediator = mediator;
	}


	[HttpGet("{id}")]
	[SwaggerOperation("Get post")]
	[ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetById(string id)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Region/{id}")]
	[SwaggerOperation("Get all posts in region")]
	[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAllInRegion(int id)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Authored/{id}")]
	[SwaggerOperation("Get all posts made by user id")]
	[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAllAuthoredBy(string id)
	{
		throw new NotImplementedException();
	}

	[HttpGet("{id}/Comments")]
	[SwaggerOperation("Get all comments of a post")]
	[ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAllComments(int id)
	{
		throw new NotImplementedException();
	}

	[HttpPost]
	[SwaggerOperation("Add a post (User only)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> Add()
	{
		throw new NotImplementedException();
	}

	//we never delete or update posts
	//the only thing is to mark it not Active (Mod only)


	[HttpPut("{id}")]
	[SwaggerOperation("Mark a post as Not Active (Moderator only)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> MarkAsInactive(string id)
	{
		throw new NotImplementedException();
	}
}
