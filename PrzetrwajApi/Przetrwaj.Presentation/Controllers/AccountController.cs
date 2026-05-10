using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Przetrwaj.Application.Commands.AccountOwn;
using Przetrwaj.Application.Commands.Confirm;
using Przetrwaj.Application.Helpers;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
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
	private readonly IMediator _mediator;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly IAuthService _authService;
	private readonly UserManager<AppUser> _userManager;
	IOptions<JwtSettings> _options;

	public AccountController(IMediator mediator, SignInManager<AppUser> signInManager, IAuthService authService, UserManager<AppUser> userManager, IOptions<JwtSettings> options)
	{
		_mediator = mediator;
		_signInManager = signInManager;
		_authService = authService;
		_userManager = userManager;
		_options = options;
	}

	[HttpGet]
	[Authorize]
	[SwaggerOperation("Gets user own details (Owner)")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetUserOwnInfo()
	{
		var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (currentUserId is null)
			return BadRequest((ExceptionCasting)new InvalidCookieException("Invalid Cookie")); // Returns a 400 User for some reason does not exist

		try
		{
			var user = await _authService.GetUserDetailsAsync(currentUserId);
			var dto = (UserWithPersonalDataDto)user;
			var roles = await _userManager.GetRolesAsync(user);
			dto.Roles = roles;
			return Ok(dto);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpPut]
	[Authorize]
	[SwaggerOperation("Updates user own account (Owner)")]
	[ProducesResponseType(typeof(UserWithPersonalDataDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status409Conflict)]
	public async Task<IActionResult> UpdateUserAccount(UpdateAccountCommand updateAccount, CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (currentUserId is null)
			return BadRequest((ExceptionCasting)new InvalidCookieException("Invalid Cookie/Token")); // Returns a 400 User for some reason does not exist
		var requ = new UpdateAccountInternallCommand
		{
			UserId = currentUserId,
			Update = updateAccount
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


	[HttpGet("confirm-email")]
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

	[HttpGet("confirm-email-change")]
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

	[HttpPost("WIP/forgot-password")]
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

	[Authorize]
	[HttpPost("logout")]
	[SwaggerOperation("Logout (Owner)")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Logout()
	{
		var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
		if (UserId is null)
			return BadRequest((ExceptionCasting)new InvalidCookieException("Invalid Cookie")); // Returns a 400 User for some reason does not exist

		await _mediator.Send(new LogoutCommand { UserId = UserId, TokenId = jti });

		await _signInManager.SignOutAsync();    //drop all potential cookies
		return NoContent();
	}

	[Authorize]
	[HttpPost("logout-all-sessions")]
	[SwaggerOperation("Logout all sessions (Owner)")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> LogoutAllSessions()
	{
		var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (UserId is null)
			return BadRequest((ExceptionCasting)new InvalidCookieException("Invalid Cookie")); // Returns a 400 User for some reason does not exist

		await _mediator.Send(new LogoutAllSesionsCommand { UserId = UserId });

		await _signInManager.SignOutAsync();    //drop all potential cookies
		return NoContent();
	}

	[HttpPost("refresh-token")]
	[SwaggerOperation("Refresh user JWT token providing the RefreshToken (Owner)")]
	[ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> RefreshToken(
		[FromAuthorizationHeader] List<string> Authorizations,
		[FromBody] RefreshTokenCommand request,
		CancellationToken ct
	)
	{
		//this endpoint is not Authorized as that would require a valid Token in the first place
		///@see https://medium.com/@MatinGhanbari/building-a-secure-api-with-asp-net-core-jwt-and-refresh-tokens-03dac37b4055
		///for info of implementation.
		///The backend has an /auth/refreshToken
		/// If the frontend calls an api in the backend, and the token is expired, it returns 401 unauthorized
		/// The frontend recognizes that it received a 401, and automatically calls /auth/refreshToken passing the tokens, 
		///  if that one returns 200 Ok, then it redoes the first api call that originally returned 401.
		/// If Both return 401, then the frontend redirects the user to the login page.

		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		var authorizationHelper = new AuthorizationHelper(_options);
		try
		{
			var claims = authorizationHelper.GetPrincipalClaimsFromTokens(Authorizations, ValidateLifetime: false);

			var UserId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
			var jti = claims.FindFirstValue(JwtRegisteredClaimNames.Jti);

			var tokens = await _mediator.Send(new RefreshTokenInternalCommand
			{
				RefreshToken = request.RefreshToken,
				Jti = jti,
				UserId = UserId
			}, ct);
			return Ok(tokens);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
		catch (SecurityTokenException ex)
		{
			return BadRequest(new ExceptionCasting
			{
				StatusCode = System.Net.HttpStatusCode.BadRequest,
				Status = "error",
				Error = new ErrorDetails { Code = ex.GetType().Name, Message = ex.Message }
			});
		}
	}

}
