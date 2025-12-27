using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Users;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Controllers;


[Route("[controller]")]
[ApiController]
[Authorize(UserRoles.Admin)]
public class AdminController : Controller
{
	private readonly IMediator _mediator;

	public AdminController(IMediator mediator)
	{
		_mediator = mediator;
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

}
