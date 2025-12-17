using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Posts.Attachments;
using Przetrwaj.Application.Dtos.Posts;
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


	//[HttpPost("_Filtered")]
	//[SwaggerOperation("Get posts that match filter")]
	//[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	//[ProducesResponseType(StatusCodes.Status400BadRequest)]
	//public async Task<IActionResult> GetFiltered()
	//{
	//	throw new NotImplementedException();
	//}

	//PN priority high
	[HttpGet("{id}")]
	[SwaggerOperation("Get post with all content")]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	//[HttpGet("Region/{id}")]
	//[SwaggerOperation("Get all posts in region")]
	//[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	//[ProducesResponseType(StatusCodes.Status404NotFound)]
	//public async Task<IActionResult> GetAllInRegion(int id)
	//{
	//	throw new NotImplementedException();
	//}

	[HttpGet]
	[SwaggerOperation("Get all posts for map display.")]
	[ProducesResponseType(typeof(IEnumerable<PostMinimalCategoryRegionDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAllPosts(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	//KL priority high
	[HttpGet("Region/{id}/Danger")]
	[SwaggerOperation("Get all Danger posts in region")]
	[ProducesResponseType(typeof(IEnumerable<PostOverviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetDangerInRegion(int id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Region/{id}/Resource")]
	[SwaggerOperation("Get all Resource posts in region")]
	[ProducesResponseType(typeof(IEnumerable<PostOverviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetResourceInRegion(int id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	//[HttpGet("Region/Custom")]
	//[SwaggerOperation("Get all posts with Custom region")]
	//[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	//public async Task<IActionResult> GetAllWithCustomRegion()
	//{
	//	throw new NotImplementedException();
	//}

	[HttpGet("Authored/{id}")]
	[SwaggerOperation("Get all posts made by user id")]
	[ProducesResponseType(typeof(IEnumerable<PostOverviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllAuthoredBy(string id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}


	//[HttpGet("{id}/Comment")]
	//[SwaggerOperation("Get all comments of a post")]
	//[ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
	//[ProducesResponseType(StatusCodes.Status404NotFound)]
	//public async Task<IActionResult> GetAllComments(int id)
	//{
	//	throw new NotImplementedException();
	//}

	[HttpPost("{id}/Comment")]
	[SwaggerOperation("Add a comment to the post (User)")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> AddComment(int id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	//KL priority high
	[HttpPost("{id}/VotePositive")]
	[SwaggerOperation("Add a Positive vote to the post (User)")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)] //already voted
	public async Task<IActionResult> VotePositive(int id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	//KL priority high
	[HttpPost("{id}/VoteNegative")]
	[SwaggerOperation("Add a Negative vote to the post (User)")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)] //already voted
	public async Task<IActionResult> VoteNegative(int id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	//PN priority high
	[HttpPost("Danger")]
	[SwaggerOperation("Add a Danger post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddDanger(AddAttachment files, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	[HttpPost("Resource")]
	[SwaggerOperation("Add a Recource post (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddResource(AddAttachment files, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	//we never delete or update posts
	//the only thing is to mark it not Active (Mod only)


	[HttpPut("{id}")]
	[SwaggerOperation("Mark a post as Not Active (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> MarkAsInactive(string id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
