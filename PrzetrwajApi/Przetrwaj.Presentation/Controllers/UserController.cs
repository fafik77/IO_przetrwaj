using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Users;
using Przetrwaj.Application.Quaries.Posts;
using Przetrwaj.Application.Quaries.Users;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Domain.Models.Dtos.Posts;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Controllers;

/// <summary>
/// This is the Public General User endpoint.
/// Do not confuse this one with 'AccountController',
/// as this one does not return any sensitive data period.
/// </summary>
[Route("[controller]")]
[ApiController]
public class UserController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly IMediator _mediator;

	public UserController(UserManager<AppUser> userManager, IMediator mediator)
	{
		_userManager = userManager;
		_mediator = mediator;
	}


	[HttpGet("{id}")]
	[SwaggerOperation("Get publicly visible General data of user by id")]
	[ProducesResponseType(typeof(UserGeneralDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
	{
		try
		{
			var user = await _mediator.Send(new GetUserByIdQuery { UserId = id }, cancellationToken);
			return Ok(user);
		}
		catch (BaseException ex)
		{
			return NotFound((ExceptionCasting)ex);
		}
	}


	[HttpGet("{id}/Posts")]
	[SwaggerOperation("Get all posts made by user id")]
	[ProducesResponseType(typeof(IEnumerable<PostCompleteDataDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllPosts(string id, CancellationToken cancellationToken)
	{
		var requ = new GetAllAuthoredByQuery { AutorId = id };
		try
		{
			var res = await _mediator.Send(requ, cancellationToken);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpGet("WIP/{id}/Comments")]
	[SwaggerOperation("Get all comments made by user id")]
	[ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllComments(string id, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}


	[HttpPost("MakeModerator")]
	[SwaggerOperation("Grant Moderator role to user by Id or Email (Admin)")]
	[ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> AssignModeratorRole(MakeModeratorCommand userInfo, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid) return BadRequest(IdentityResult.Failed(new IdentityError { Description = $"{ModelState}" }));
		try
		{
			var res = await _mediator.Send(userInfo, cancellationToken);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return NotFound((ExceptionCasting)ex);
		}
	}


	[HttpPost("Ban")]
	[Authorize(UserRoles.Moderator)]
	[SwaggerOperation("Ban a User by Id or Email (Moderator, Admin)")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Ban(BanUserCommand banUserCommand, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		//get info from the cookie and send a request
		var command = new BanUserInternallCommand
		{
			UserIdOrEmail = banUserCommand.UserIdOrEmail,
			Reason = banUserCommand.Reason,
			ModeratorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
		};
		try
		{
			var res = await _mediator.Send(command, cancellationToken);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}
}
