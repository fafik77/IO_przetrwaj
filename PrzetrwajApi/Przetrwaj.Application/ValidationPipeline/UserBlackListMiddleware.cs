using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Przetrwaj.Domain.Abstractions;
using System.Security.Claims;

namespace Przetrwaj.Application.ValidationPipeline;

public class UserBlackListMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IBanCache _banCache;
	private readonly ILogoutCache _logoutCache;

	public UserBlackListMiddleware(RequestDelegate next, IBanCache banCache, ILogoutCache logoutCache)
	{
		_next = next;
		_banCache = banCache;
		_logoutCache = logoutCache;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// 1. Only check if the user is logged in
		if (context.User.Identity?.IsAuthenticated == true)
		{
			// 1.2. Check if public endpoint (no Authorize attribute)
			var endpoint = context.GetEndpoint();
			var authorizeData = endpoint?.Metadata?.GetMetadata<IAuthorizeData>();
			// If no Authorize attribute is present, it's a public endpoint
			if (authorizeData is null)
			{
				await _next(context);
				return;
			}

			var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var tokenId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId != null)
			{
				// 2. Check the memory cache
				if (_banCache.IsUserBanned(userId))
				{
					var banInfo = await _banCache.GetUserBanInfoAsync(userId);
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
					await context.Response.WriteAsJsonAsync(new
					{
						error = "Banned",
						message = $"Twoje konto zostało zablokowane dnia {banInfo.BanDate}" +
						$" przez Moderatora {banInfo.BannedBy?.Name ?? string.Empty} {banInfo.BannedBy?.Surname ?? string.Empty} z {banInfo.BannedBy?.Region?.Name ?? string.Empty}\n" +
						$"Powód: {banInfo.BanReason}.\nSkontaktuj się z moderatorem lub administratorem."
					});
					return; // Stop the request here
				}
				else if (_logoutCache.IsLogedOut(userId, tokenId))
				{
					return; // Stop the request here
				}
			}
		}
		await _next(context);
	}
}
