using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using System.Text;

namespace Przetrwaj.Application.Services;

public class ModeratorRolePendingNotificationService : BackgroundService
{
	private readonly ILogger<ModeratorRolePendingNotificationService> _logger;
	private readonly IServiceProvider _services;

	public ModeratorRolePendingNotificationService(ILogger<ModeratorRolePendingNotificationService> logger, IServiceProvider services)
	{
		_logger = logger;
		_services = services;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			// 1. Calculate the delay until 08:00
			var now = DateTime.Now;	//Time is the system time (location dependant) it's not UTC GMT+0
			var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
			// If it's already past 08:00 today, schedule for tomorrow
			if (now >= nextRunTime)
				nextRunTime = nextRunTime.AddDays(1);
			var delay = nextRunTime - now;
			_logger.LogInformation("Service scheduled. Next run at {time} (Waiting {delay:F1} hours)", nextRunTime, delay.TotalHours);
			await Task.Delay(delay, stoppingToken);
			
			_logger.LogInformation("Notification Service working at: {time}", DateTimeOffset.Now);
			await NotifyAdminsOfPendingModerators(stoppingToken);
		}
	}

	protected async Task NotifyAdminsOfPendingModerators(CancellationToken cancellationToken)
	{
		using var scope = _services.CreateScope();
		var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
		var _emailSenderMultiple = scope.ServiceProvider.GetRequiredService<IEmailSenderMultiple>();
		var _frontEndSettings = scope.ServiceProvider.GetRequiredService<IOptions<FrontEndSettings>>().Value;
		var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
		//get all users that want to be a Mod
		var usersForMods = await _userRepository.GetModPendingUsersRWAsync(cancellationToken);
		var usersForModsCount = usersForMods.Count();
		if (usersForModsCount != 0)
		{
			var admins = await _userManager.GetUsersInRoleAsync(UserRoles.Admin);
			var adminsEmails = admins.Select(x => x.Email!).ToList();
			// 1. Build the HTML Table
			var sb = new StringBuilder();
			sb.Append("<h2>Oczekujący na rolę Moderatora</h2>");
			sb.Append($"<p>Następujący użytkownicy({usersForModsCount} użytkowników) zgłosili chęć moderacji:</p>");
			sb.Append("<table border='1' cellpadding='10' style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif;'>");
			sb.Append("<tr style='background-color: #f2f2f2;'>");
			sb.Append("<th>Email</th><th>Nazwa Regionu</th><th>ID Regionu</th>");
			sb.Append("</tr>");
			foreach (var user in usersForMods)
			{
				sb.Append("<tr>");
				sb.Append($"<td>{user.Email}</td>");
				sb.Append($"<td>{user.IdRegionNavigation.Name}</td>");
				sb.Append($"<td>{user.IdRegion}</td>");
				sb.Append("</tr>");
			}
			sb.Append("</table>");
			sb.Append($"<p>Zaloguj się do panelu administratora, aby przejrzeć te zgłoszenia: <a href='{_frontEndSettings.Url}/admin/moderators'>Panel Admina</a></p>");
			sb.Append($"<br><br><p style='color: gray; font-size: 12px;'>Ten email został wysłany automatycznie z serwisu <a href='{_frontEndSettings.Url}'>Przetrwaj.pl</a> prosimy na niego nie odpowiadać.</p>");
			// 2. Send the email using BCC for admin privacy
			await _emailSenderMultiple.SendBlindEmailToMultipleAsync(
				adminsEmails,
				subject: $"Użytkownicy Przetrwaj.pl oczekują roli Moderatora ({usersForModsCount})",
				htmlMessage: sb.ToString()
			);
		}
	}
}
