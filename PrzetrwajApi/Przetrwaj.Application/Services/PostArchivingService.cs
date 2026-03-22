using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Services;

/// <summary>
/// This service marks Posts as Inactive when the amount of downvotes is greater than upvotes
/// </summary>
public class PostArchivingService : BackgroundService
{
	private readonly ILogger<PostArchivingService> _logger;
	private readonly IServiceProvider _services;
	private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30); // Check every X Minutes

	public PostArchivingService(ILogger<PostArchivingService> logger, IServiceProvider services)
	{
		_logger = logger;
		_services = services;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken);
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Post Archiving Service working at: {time}", DateTimeOffset.Now);
			await MarkPostsAsInactive(stoppingToken);
			await Task.Delay(_checkInterval, stoppingToken);
		}
	}

	private async Task MarkPostsAsInactive(CancellationToken ct)
	{
		using var scope = _services.CreateScope();
		var postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();
		///change this method to return list<post ids> of inactive Posts
		var postsArchived = await postRepository.ArchiveInactivePostsAsync(ct);

		if (postsArchived != 0)
			_logger.LogInformation("Archived {postsArchived} posts.", postsArchived);
	}
}
