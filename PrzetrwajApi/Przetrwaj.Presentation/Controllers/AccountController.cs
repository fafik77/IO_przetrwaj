using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.AccountOwn;
using Przetrwaj.Application.Commands.Confirm;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Controllers;


/// <summary>
/// This is the Personal Account enpoint, only Owner can access those endpoint, 
/// here we do not return sensitive data, or Only Moderator+ has acces to it.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AccountController : Controller
{
	private const string AuthenticationCookie = "cookie";
	private readonly IMediator _mediator;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;
	private readonly IAuthService _authService;
	private readonly RoleManager<IdentityRole> _roleManager;
	public AccountController(IMediator mediator, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IAuthService authService, RoleManager<IdentityRole> roleManager)
	{
		_mediator = mediator;
		_signInManager = signInManager;
		_userManager = userManager;
		_authService = authService;
		_roleManager = roleManager;
	}


	[HttpGet]
	[SwaggerOperation("Gets user own details (Owner only)")]
	[Authorize]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetUserOwnInfo()
	{
		var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (currentUserId is null)
			return NotFound(); // Returns a 404 User for some reason does not exist

		var user = await _authService.GetUserDetailsAsync(currentUserId);

		if (user == null)
			return NotFound($"Not Found: {currentUserId}");

		var dto = (UserWithPersonalDataDto)user;
		return Ok(dto);
	}

	[HttpPut]
	[SwaggerOperation("Updates user own account (Owner only)")]
	[Authorize]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateUserAccount(UpdateAccountCommand updateAccount)
	{
		var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (currentUserId is null)
			return NotFound(); // Returns a 404 User for some reason does not exist
		var requ = new UpdateAccountInternalCommand
		{
			UserId = currentUserId,
			IdRegion = updateAccount.IdRegion,
			Name = updateAccount.Name,
			Surname = updateAccount.Surname,
		};

		var res = await _mediator.Send(requ);
		return Ok(res);
	}


	[HttpGet("ConfirmEmail")]
	[SwaggerOperation("Confirm Email using the code attached in email")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ConfirmEmail(string userId, string code)
	{
		if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
			return BadRequest("Invalid email confirmation request.");

		var command = new ConfirmEmailCommand { userId = userId, code = code };
		var res = await _mediator.Send(command);
		if (res == null) return BadRequest("Invalid email confirmation request.");
		return Ok(res);
	}

	[HttpPost("Logout")]
	[SwaggerOperation("Logout")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();
		await HttpContext.SignOutAsync(AuthenticationCookie);
		return NoContent();
	}


}
