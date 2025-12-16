using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Users;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Dtos.Posts;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
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


	[HttpGet("WIP/{id}")]
	[SwaggerOperation("Get publicly visible General data of user by id")]
	[ProducesResponseType(typeof(IEnumerable<PostCompleteDataDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(string id)
	{
		throw new NotImplementedException();
	}


	[HttpGet("WIP/{id}/Posts")]
	[SwaggerOperation("Get all posts made by user id")]
	[ProducesResponseType(typeof(IEnumerable<PostCompleteDataDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllPosts(string id)
	{
		throw new NotImplementedException();
	}

	[HttpGet("WIP/{id}/Comments")]
	[SwaggerOperation("Get all comments made by user id")]
	[ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetAllComments(string id)
	{
		throw new NotImplementedException();
	}




	[HttpPost("MakeModerator")]
	[Authorize(UserRoles.Admin)]
	[SwaggerOperation("Grant Moderator role to user by Id or Email (Admin only)")]
	[ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> AssignModeratorRole(MakeModeratorCommand userInfo)
	{
		if (!ModelState.IsValid) return BadRequest(IdentityResult.Failed(new IdentityError { Description = $"{ModelState}" }));
		try
		{
			var res = await _mediator.Send(userInfo);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return NotFound((ExceptionCasting)ex);
		}
	}

	[HttpPost("Ban")]
	[Authorize(UserRoles.Moderator)]
	[SwaggerOperation("Ban a user by Id or Email (Moderator only)")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Ban(BanUserCommand banUserCommand)
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
			var res = await _mediator.Send(command);
			return Ok(res);
		}
		catch (BaseException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
		{
			return NotFound((ExceptionCasting)ex);
		}
	}
}
