using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models;
using System.Security.Claims;

namespace Przetrwaj.Application.ValidationPipeline;

public class BanMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IBanCache _banCache;

	public BanMiddleware(RequestDelegate next, IBanCache banCache)
	{
		_next = next;
		_banCache = banCache;
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
						$" przez Moderatora {banInfo.BannedBy.Name} {banInfo.BannedBy.Surname} z {banInfo.BannedBy.Region?.Name}\n" +
						$"Powód: {banInfo.BanReason}.\nSkontaktuj się z administratorem."
					});
					return; // Stop the request here
				}
			}
		}
		await _next(context);
	}
}
