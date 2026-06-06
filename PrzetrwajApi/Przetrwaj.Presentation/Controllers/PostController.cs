using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Commands.Posts;
using Przetrwaj.Application.Commands.Posts.Attachments;
using Przetrwaj.Application.Helpers;
using Przetrwaj.Application.Quaries.Posts;
using Przetrwaj.Application.Queries.Posts;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Domain.Models.Dtos.Posts;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]s")]
[ApiController]
[Produces("application/json")]
public partial class PostController : Controller
{
	private readonly IMediator _mediator;
	private readonly IOptions<JwtSettings> _jwtOptions;

	public PostController(IMediator mediator, IOptions<JwtSettings> jwtOptions)
	{
		_mediator = mediator;
		_jwtOptions = jwtOptions;
	}


	[HttpGet("{id}")]
	[SwaggerOperation("Get post with all content. (contains MyVote)")]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(
		[FromAuthorizationHeader] List<string> Authorizations,
		[FromRoute] string id,
		CancellationToken CT)
	{
		string? userId = null;
		try
		{
			if (Authorizations.Count == 1)
			{
				var helper = new AuthorizationHelper(_jwtOptions);
				var claims = helper.GetPrincipalClaimsFromTokens(Authorizations);
				userId = AuthorizationHelper.GetUserId(claims);
			}
			var post = await _mediator.Send(new GetPostByIdQuery { Id = id, UserId = userId }, CT);
			return Ok(post);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpGet("map")]
	[SwaggerOperation("Get all posts for map display.")]
	[ProducesResponseType(typeof(IEnumerable<PostMinimalCategoryRegion>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAllPosts(CancellationToken CT)
	{
		var posts = await _mediator.Send(new GetAllPostsMinimalQuery(), CT);
		return Ok(posts);
	}

	[HttpGet]
	[SwaggerOperation("Get all matching posts, sort in order of relevance")]
	[ProducesResponseType(typeof(IEnumerable<PostOverviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetMatchingPosts(
		[FromQuery] int RegionId,
		[FromQuery] int? Impediment,
		[FromQuery] RegionPrecision? MaxLevel,
		[FromQuery] CategoryTypeFilter? Category,
		CancellationToken CT)
	{
		var request = new GetAllMatchingPostsQuery
		{
			MatchingPostsFilter = new()
			{
				RegionId = RegionId,
				Impediment = Impediment ?? 0,
				MaxLevel = MaxLevel,
				CategoryFilter = Category
			}
		};
		try
		{
			var posts = await _mediator.Send(request, CT);
			return Ok(posts);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}



	[HttpGet("authored/{id}")]
	[SwaggerOperation("Get all posts made by user id")]
	[ProducesResponseType(typeof(IEnumerable<PostOverviewDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllAuthoredBy(string id, CancellationToken CT)
	{
		var requ = new GetAllAuthoredByQuery { AutorId = id };
		try
		{
			var res = await _mediator.Send(requ, CT);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpPost("{id}/comment")]
	[SwaggerOperation("Add a comment to the post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> AddComment(string id, AddCommentCommand command, CancellationToken CT)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var internalCommand = new AddCommentInternalCommand
		{
			Comment = command.Comment,
			IdPost = id,
			// Set user from cookie
			IdAutor = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
		};
		try
		{
			var res = await _mediator.Send(internalCommand, CT);
			return CreatedAtAction(nameof(GetById), new { id }, res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	//KL Done
	[HttpPost("{id}/vote-positive")]
	[SwaggerOperation("Add a Positive vote to the post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(VoteDto), StatusCodes.Status409Conflict)] //already voted
	public async Task<IActionResult> VotePositive(string id, CancellationToken CT)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			await _mediator.Send(new VoteOnPostCommand
			{
				IdPost = id,
				IdUser = userId,
				IsUpvote = true
			}, CT);

			return NoContent();
		}
		catch (AlreadyVotedException ex)
		{
			return Conflict(ex.Vote);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	//KL Done
	[HttpPost("{id}/vote-negative")]
	[SwaggerOperation("Add a Negative vote to the post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(VoteDto), StatusCodes.Status409Conflict)] //already voted
	public async Task<IActionResult> VoteNegative(string id, CancellationToken CT)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			await _mediator.Send(new VoteOnPostCommand
			{
				IdPost = id,
				IdUser = userId,
				IsUpvote = false
			}, CT);

			return NoContent();
		}
		catch (AlreadyVotedException ex)
		{
			return Conflict(ex.Vote);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	//KL Done, PN exposed
	[HttpGet("{id}/vote")]
	[SwaggerOperation("Get user Vote status on Post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(typeof(VoteDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetVote(string id, CancellationToken CT)
	{
		var requ = new GetUserVoteQuery
		{
			PostId = id,
			UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
		};
		try
		{
			var res = await _mediator.Send(requ, CT);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}


	//ToDo: implement CustomCategory checking (allow it only if Category.id/Name == inne)
	[HttpPost("danger")]
	[SwaggerOperation("Add a Danger post (User)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddDanger(AddPostCommand newPost, CancellationToken CT)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var postI = new AddDangerInternallCommand
		{
			AddPostCommand = newPost,
			// Set user from cookie
			IdAutor = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
			ClaimsPrincipal = User,
		};
		try
		{
			var res = await _mediator.Send(postI, CT);
			return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	//ToDo: implement CustomCategory checking (allow it only if Category.id/Name == inne)
	[HttpPost("resource")]
	[SwaggerOperation("Add a Resource post (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(typeof(PostCompleteDataDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddResource(AddPostCommand newPost, CancellationToken CT)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var postI = new AddResourceInternallCommand
		{
			AddPostCommand = newPost,
			// Set user from cookie
			IdAutor = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
			ClaimsPrincipal = User,
		};
		try
		{
			var res = await _mediator.Send(postI, CT);
			return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpPost("{id}/attachment")]
	[SwaggerOperation("Add Attachments to post (Owner of the post)(max 50 MiB request)")]
	[Authorize(UserRoles.User)]
	[ProducesResponseType(typeof(AddAttachmentsResult), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(AddAttachmentsResult), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(AddAttachmentsResult), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[RequestFormLimits(MultipartBodyLengthLimit = 50 << 20)] //up to 50 MB
	[RequestSizeLimit(50 << 20)] //up to 50 MB
	public async Task<IActionResult> AddAttachment(string id, [FromForm] AddAttachments attachments, CancellationToken CT)
	{
		var req = new AddAttachmentsInternal
		{
			IdPost = id,
			Items = attachments.Items,
			// Set user from cookie
			IdUser = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
		};
		try
		{
			var res = await _mediator.Send(req, CT);
			return StatusCode((int)res.StatusCode, res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	//we never delete or update posts
	//the only thing is to mark it not Active (Mod only)


	[HttpPut("{id}/mark-inactive")]
	[SwaggerOperation("Mark a post as Not Active (Moderator)")]
	[Authorize(UserRoles.Moderator)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> MarkAsInactive(string id, CancellationToken CT)
	{
		var requ = new MarkPostAsInactiveCommand { PostId = id };
		try
		{
			await _mediator.Send(requ, CT);
			return NoContent();
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}
}
