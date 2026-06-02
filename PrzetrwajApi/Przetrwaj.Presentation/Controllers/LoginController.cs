using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Commands.Login;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Exceptions.Auth;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace Przetrwaj.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public class LoginController : Controller
{
	private readonly IMediator _mediator;
	private readonly FrontEndSettings _frontEndSettings;

	public LoginController(IMediator mediator, IOptions<FrontEndSettings> options)
	{
		_mediator = mediator;
		_frontEndSettings = options.Value;
	}

	[HttpPost("email")]
	[SwaggerOperation("Login using email. 418 with info if user is banned")]
	[ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ExceptionCasting), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(BanInfo), StatusCodes.Status418ImATeapot)]
	public async Task<IActionResult> EmailLogin([FromBody] LoginEmailCommand model)
	{
		if (!ModelState.IsValid) return BadRequest((ExceptionCasting)ModelState);
		try
		{
			var result = await _mediator.Send(model);
			return Ok(result);
		}
		catch (UserBannedException ex)
		{
			return StatusCode(StatusCodes.Status418ImATeapot, ex.BanInfo);
		}
		catch (BaseException ex)
		{
			return StatusCode((int)ex.HttpStatusCode, (ExceptionCasting)ex);
		}
	}

	[HttpGet("google")]
	[SwaggerOperation("Login using Google gmail")]
	[ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status302Found)]
	[ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(BanInfo), StatusCodes.Status418ImATeapot)]
	public async Task<IActionResult> GoogleLogin(string? returnUrl = null)
	{
		if (string.IsNullOrEmpty(returnUrl))
			returnUrl = Request.Headers.Referer.ToString(); //auto fill in from requester site

		var redirectUrl = Url.Action(nameof(GoogleResponse), new { returnUrl });
		if (redirectUrl is null) return NotFound("GoogleResponse endpoint not found");
		var properties = await _mediator.Send(new GoogleLoginCommand(redirectUrl));
		return Challenge(properties, "Google");
	}

	[HttpGet("google-response")]
	[SwaggerOperation("Login using Google gmail. Redirects back to frontend /login-callback")]
	[ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status302Found)]
	public async Task<IActionResult> GoogleResponse(string? returnUrl = null)
	{
		// SECURITY CHECK: Ensure the URL is local or from allowed domains
		if (string.IsNullOrEmpty(returnUrl) || !IsUrlSafe(returnUrl))
		{
			returnUrl = _frontEndSettings.Url; // Fallback to frontEnd Url
		}
		if (!returnUrl.EndsWith('/')) returnUrl += "/";
		try
		{
			var result = await _mediator.Send(new GoogleLoginResponseCommand());

			string safeToken = Uri.EscapeDataString(result.Token ?? string.Empty);
			string safeRefreshToken = Uri.EscapeDataString(result.RefreshToken ?? string.Empty);
			string safeSuccess = result.Success.ToString().ToLower();
			return Redirect($"{returnUrl}login-callback?token={safeToken}&refreshToken={safeRefreshToken}&success={safeSuccess}");
		}
		catch (UserBannedException ex)
		{
			return Redirect($"{returnUrl}login-error/banned?info={Uri.EscapeDataString(ex.BanInfo.ToString())}");
		}
		catch (BaseException ex)
		{
			return Redirect($"{returnUrl}login-error/failed?msg={Uri.EscapeDataString(ex.Message)}");
		}
	}


	// Helper to prevent Open Redirect attacks
	private bool IsUrlSafe(string url)
	{
		// Check if it starts with front-end domain or is a local path
		return url.StartsWith(_frontEndSettings.Url) || url.StartsWith("https://localhost:");
	}
}
