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

	private async Task MarkPostsAsInactive(CancellationToken cancellationToken)
	{
		using var scope = _services.CreateScope();
		var postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();
		var posts = await postRepository.GetAllWithVotesStatusROAsync(cancellationToken);

		var idsToDeactivate = posts
			.Where(p => p.VoteNegative > p.VotePositive)
			.Select(p => p.Id)
			.ToList();
		foreach (var chunk in idsToDeactivate.Chunk(1000))
		{
			//Perform ONE atomic database update for a chunk of identified posts
			await postRepository.SetInactiveBulkAsync(chunk, cancellationToken);
		}
		if (idsToDeactivate.Count != 0)
			_logger.LogInformation("Archived {Count} posts.", idsToDeactivate.Count);
	}
}