using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.AccountOwn;
using Przetrwaj.Application.Commands.Confirm;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Controllers;


/// <summary>
/// This is the Personal Account enpoint, only Owner can access those endpoint, 
/// here we do not return sensitive data, or Only Moderator+ has acces to it.
/// </summary>
[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public class AccountController : Controller
{
	//private const string AuthenticationCookie = "cookie";
	private readonly IMediator _mediator;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly IAuthService _authService;
	private readonly UserManager<AppUser> _userManager;

	public AccountController(IMediator mediator, SignInManager<AppUser> signInManager, IAuthService authService, UserManager<AppUser> userManager)
	{
		_mediator = mediator;
		_signInManager = signInManager;
		_authService = authService;
		_userManager = userManager;
	}


	[HttpGet]
	[SwaggerOperation("Gets user own details (Owner only)")]
	[Authorize]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetUserOwnInfo()
	{
		var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (currentUserId is null)
			return NotFound((ExceptionCasting)new InvalidCookieException("Invalid Cookie")); // Returns a 404 User for some reason does not exist

		try
		{
			var user = await _authService.GetUserDetailsAsync(currentUserId);
			var dto = (UserWithPersonalDataDto)user;
			var roles = await _userManager.GetRolesAsync(user);
			dto.Role = string.Join(", ", roles);
			return Ok(dto);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpPut]
	[SwaggerOperation("Updates user own account (Owner only)")]
	[Authorize]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status409Conflict)]
	public async Task<IActionResult> UpdateUserAccount(UpdateAccountCommand updateAccount, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (currentUserId is null)
			return NotFound((ExceptionCasting)new InvalidCookieException("Invalid Cookie")); // Returns a 404 User for some reason does not exist
		var requ = new UpdateAccountInternallCommand
		{
			UserId = currentUserId,
			IdRegion = updateAccount.IdRegion,
			Name = updateAccount.Name,
			Surname = updateAccount.Surname,
			Email = updateAccount.Email,
			NewPassword = updateAccount.NewPassword,
			OldPassword = updateAccount.OldPassword,
		};
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


	[HttpGet("ConfirmEmail")]
	[SwaggerOperation("Confirm Email using the code attached in email")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ConfirmEmail(string userId, string code, CancellationToken cancellationToken)
	{
		if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
			return BadRequest((ExceptionCasting)new InvalidConfirmationException("Invalid email confirmation request."));

		var command = new ConfirmEmailCommand { UserId = userId, Code = code };
		try
		{
			var res = await _mediator.Send(command, cancellationToken);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return BadRequest((ExceptionCasting)ex);
		}
	}

	[HttpGet("ConfirmEmailChange")]
	[SwaggerOperation("Confirm Email Change using the code attached in the email")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ConfirmEmailChange(string userId, string code, string newEmail, CancellationToken cancellationToken)
	{
		if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(newEmail))
			return BadRequest((ExceptionCasting)new InvalidConfirmationException("Invalid change email confirmation request."));

		var command = new ConfirmEmailChangeCommand { UserId = userId, Code = code, NewEmail = newEmail };
		try
		{
			var res = await _mediator.Send(command, cancellationToken);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return BadRequest((ExceptionCasting)ex);
		}
	}

	[HttpPost("WIP/ForgotPassword")]
	[SwaggerOperation("Forgot password, request a reset")]
	[ProducesResponseType(typeof(UserGeneralDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken cancellationToken)
	{
		return StatusCode(statusCode: StatusCodes.Status501NotImplemented);
		if (!ModelState.IsValid)
			return BadRequest((ExceptionCasting)ModelState);
		try
		{
			var res = await _mediator.Send(command, cancellationToken);
			return Ok(res);
		}
		catch (BaseException ex)
		{
			return BadRequest((ExceptionCasting)ex);
		}
	}

	[HttpPost("Logout")]
	[SwaggerOperation("Logout")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();
		//await HttpContext.SignOutAsync(AuthenticationCookie);
		return NoContent();
	}


}
