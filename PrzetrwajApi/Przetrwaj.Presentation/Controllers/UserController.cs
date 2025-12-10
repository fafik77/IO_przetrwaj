using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Users;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Controllers;

/// <summary>
/// This is the Public General User endpoint.
/// Do not confuse this one with 'AccountController',
/// as this one does not return any sensitive data period.
/// </summary>
[Route("WIP/[controller]")]
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
	[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(string id)
	{
		throw new NotImplementedException();
	}


	[HttpGet("{id}/Posts")]
	[SwaggerOperation("Get all posts made by user id")]
	[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllPosts(string id)
	{
		throw new NotImplementedException();
	}

	[HttpGet("{id}/Comments")]
	[SwaggerOperation("Get all comments made by user id")]
	[ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllComments(string id)
	{
		throw new NotImplementedException();
	}




	[HttpPost("MakeModerator")]
	[Authorize(UserRoles.Admin)]
	[SwaggerOperation("Grant Moderator role to user by Id or Email (Admin only)")]
	[ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
	public async Task<IdentityResult> AssignModeratorRole(MakeModeratorCommand userInfo)
	{
		if (!ModelState.IsValid) return IdentityResult.Failed(new IdentityError { Description = $"{ModelState}" });
		var res = await _mediator.Send(userInfo);
		return res;
	}

	[HttpPost("Ban")]
	[Authorize(UserRoles.Moderator)]
	[SwaggerOperation("Ban a user by Id or Email (Moderator only)")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Ban(BanUserCommand banUserCommand)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);
		//get info from the cookie and send a request
		var command = new BanUserInternallCommand
		{
			UserIdOrEmail = banUserCommand.UserIdOrEmail,
			Reason = banUserCommand.Reason,
			ModeratorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
		};
		try
		{
			var res = await _mediator.Send(command);
			return Ok(res);
		}
		catch (UserNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}
}
