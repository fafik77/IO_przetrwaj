using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Services;

/// <summary>
/// This service removes expired JWT tokens (and refresh tokens). runs every 8 hours
/// </summary>
public class ExpiredTokenCleanupService : BackgroundService
{
	private readonly ILogger<ExpiredTokenCleanupService> _logger;
	private readonly IServiceProvider _services;
	private readonly TimeSpan _checkInterval = TimeSpan.FromHours(8); // Check every X hours

	public ExpiredTokenCleanupService(IServiceProvider services, ILogger<ExpiredTokenCleanupService> logger)
	{
		_services = services;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Expired (JWT)Token Cleanup Service running.");
		await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
		while (!stoppingToken.IsCancellationRequested)
		{
			await RemoveExpiredTokensAsync(stoppingToken);
			await Task.Delay(_checkInterval, stoppingToken);
		}
	}

	private async Task RemoveExpiredTokensAsync(CancellationToken stoppingToken)
	{
		using var scope = _services.CreateScope();
		var JwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
		DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
		await JwtService.DeleteEntriesOlderThanAsync(dateTimeOffset, stoppingToken);
	}
}