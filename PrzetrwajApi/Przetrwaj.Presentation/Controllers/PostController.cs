using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Posts;
using Przetrwaj.Application.Commands.Posts.Attachments;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Dtos.Posts;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models.Dtos.Posts;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Controllers;

[Route("WIP/[controller]")]
[ApiController]
public partial class PostController : Controller
{
	private readonly IMediator _mediator;

	public PostController(IMediator mediator)
	{
		_mediator = mediator;
	}


	//PN priority high
	[HttpGet("{id}")]
	[SwaggerOperation("Get post with all content")]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(string id, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	[SwaggerOperation("Get all posts for map display.")]
	[ProducesResponseType(typeof(IEnumerable<PostMinimalCategoryRegionDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAllPosts(CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	//KL priority high
	[HttpGet("Region/{id}/Danger")]
	[SwaggerOperation("Get all Danger posts in region")]
	[ProducesResponseType(typeof(IEnumerable<PostOverviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetDangerInRegion(int id, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Region/{id}/Resource")]
	[SwaggerOperation("Get all Resource posts in region")]
	[ProducesResponseType(typeof(IEnumerable<PostOverviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetResourceInRegion(int id, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	[HttpGet("Authored/{id}")]
	[SwaggerOperation("Get all posts made by user id")]
	[ProducesResponseType(typeof(IEnumerable<PostOverviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllAuthoredBy(string id, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	[HttpPost("{id}/Comment")]
	[SwaggerOperation("Add a comment to the post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> AddComment(string id, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	//KL priority high
	[HttpPost("{id}/VotePositive")]
	[SwaggerOperation("Add a Positive vote to the post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status409Conflict)] //already voted
	public async Task<IActionResult> VotePositive(string id, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	//KL priority high
	[HttpPost("{id}/VoteNegative")]
	[SwaggerOperation("Add a Negative vote to the post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status409Conflict)] //already voted
	public async Task<IActionResult> VoteNegative(string id, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	//PN priority high
	[HttpPost("Danger")]
	[SwaggerOperation("Add a Danger post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddDanger(AddDangerCommand newPost, CancellationToken CT)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var postI = new AddDangerInternallCommand
		{
			Title = newPost.Title,
			Description = newPost.Description,
			IdCategory = newPost.IdCategory,
			CustomCategory = newPost.CustomCategory,
			IdRegion = newPost.IdRegion,
			//AlternateDescriptions = newPost.AlternateDescriptions,
			//Files = newPost.Files,
			// Set user from cookie
			IdAutor = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
		};
		try
		{
			var res = await _mediator.Send(postI, CT);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return BadRequest((ExceptionCasting)ex);
		}
	}

	[HttpPost("Resource")]
	[SwaggerOperation("Add a Recource post (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddResource(AddAttachment files, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	//PN priority low
	[HttpPost("{id}/Attachment")]
	[SwaggerOperation("Add Attachments to post (Owner of the post)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(typeof(AttachmentDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddAttachment(string id, AddAttachment attachments, CancellationToken CT)
	{
		throw new NotImplementedException();
	}

	//we never delete or update posts
	//the only thing is to mark it not Active (Mod only)


	[HttpPut("{id}")]
	[SwaggerOperation("Mark a post as Not Active (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> MarkAsInactive(string id, CancellationToken CT)
	{
		throw new NotImplementedException();
	}
}
