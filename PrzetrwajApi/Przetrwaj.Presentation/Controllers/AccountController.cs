using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Commands.Confirm;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Przetrwaj.Presentation.Controllers;

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


	[HttpGet("{idEmail}")]
	[SwaggerOperation("Gets user own details")]
	[Authorize]
	[ProducesResponseType(typeof(RegisteredUserDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetUserInfoByIdEmail(string idEmail)
	{
		bool isSelf = false;
		var currentUserIdEmail = string.Empty;
		// 1. Get the ID of the currently logged-in user
		// The ClaimTypes.NameIdentifier holds the user's Id from the Identity system.
		if (idEmail.Contains("@")) //email
			currentUserIdEmail = User.FindFirstValue(ClaimTypes.Email);
		else //id
			currentUserIdEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
		// --- 2. Authorization Check ---
		// Check 2a: Is the requesting user the SAME user? (Self-access)
		isSelf = string.Equals(currentUserIdEmail, idEmail, StringComparison.OrdinalIgnoreCase);
		// Check 2b: Does the requesting user have an Administrator role? (Admin access)
		bool isAdmin = User.IsInRole(UserRoles.Admin);

		// If the user is neither accessing their own profile NOR an admin, deny access.
		if (!(isSelf || isAdmin))
			return Forbid(); // Returns a 403 Forbidden status

		var user = await _authService.GetUserDetailsAsync(idEmail);

		if (user == null) return NotFound($"Not Found: {idEmail}");

		var dto = (RegisteredUserDto)user;
		return Ok(dto);
	}


	[HttpGet("ConfirmEmail")]
	[SwaggerOperation("Confirm Email")]
	[ProducesResponseType(typeof(RegisteredUserDto), StatusCodes.Status200OK)]
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


	[HttpPost("MakeModerator")]
	[Authorize("Admin")]
	[SwaggerOperation("Admin grants Moderator role to user by Id or Email")]
	[ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
	public async Task<IdentityResult> AssignModeratorRole(string? userId, string? userEmail)
	{
		var user = userId == null ? null : await _userManager.FindByIdAsync(userId);
		if (user == null && userEmail != null)
			user = await _userManager.FindByEmailAsync(userEmail);

		if (user == null)
			return IdentityResult.Failed(new IdentityError { Description = "User not found." });
		//await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));		//seeding roles
		//await _roleManager.CreateAsync(new IdentityRole(UserRoles.Moderator));
		//await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

		// Add the user to the Moderator role
		var result = await _userManager.AddToRoleAsync(user, UserRoles.Moderator);

		return result;
	}
}
