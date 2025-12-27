using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Abstractions;

public interface IStatisticsService
{
	Task<StatisticsDto> GetCachedOrFetchStatisticsAsync(CancellationToken cancellationToken);
	Task<StatisticsDto?> GetCachedStatisticsOnlyAsync(CancellationToken cancellationToken);
}
