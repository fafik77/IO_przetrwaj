using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Services;

public class UnconfirmedUserCleanupService : BackgroundService
{
	private readonly ILogger<UnconfirmedUserCleanupService> _logger;
	private readonly IServiceProvider _services;
	private readonly TimeSpan _checkInterval = TimeSpan.FromHours(8); // Check every X hours
	private readonly TimeSpan _cutoffTime = TimeSpan.FromHours(24);   // Delete users older than 24 hours

	public UnconfirmedUserCleanupService(
		ILogger<UnconfirmedUserCleanupService> logger,
		IServiceProvider services)
	{
		_logger = logger;
		_services = services;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Unconfirmed User Cleanup Service running.");
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Unconfirmed User Cleanup Service working at: {time}", DateTimeOffset.Now);
			await RemoveStaleUsersAsync();
			await Task.Delay(_checkInterval, stoppingToken);
		}
		_logger.LogInformation("Unconfirmed User Cleanup Service stopping.");
	}

	private async Task RemoveStaleUsersAsync()
	{
		using var scope = _services.CreateScope();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
		var cutoffDate = DateTimeOffset.UtcNow.Subtract(_cutoffTime);
		// Fetch users: Email unconfirmed AND RegistrationDate (or similar field) is older than 24 hours
		// ASP.NET Identity's ApplicationUser doesn't have a default RegistrationDate, so you must add it.
		var staleUsers = userManager.Users
			.Where(u => !u.EmailConfirmed && u.RegistrationDate < cutoffDate) // Assuming you added RegistrationDate to ApplicationUser
			.ToList();
		if (staleUsers.Count == 0)
		{
			//_logger.LogInformation("No stale unconfirmed users found.");
			return;
		}
		_logger.LogInformation("Found {count} stale unconfirmed users to delete.", staleUsers.Count);
		foreach (var user in staleUsers)
		{
			var result = await userManager.DeleteAsync(user);
			if (result.Succeeded)
			{
				//_logger.LogWarning("Successfully deleted stale unconfirmed user: {email}", user.Email);
			}
			else
			{
				_logger.LogError("Failed to delete stale unconfirmed user: {email}. Errors: {errors}",
					user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
			}
		}
	}
}
