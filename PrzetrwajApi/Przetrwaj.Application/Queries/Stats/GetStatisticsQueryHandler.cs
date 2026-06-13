using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Stats;

public class GetStatisticsQueryHandler : IQueryHandler<GetStatisticsQuery, StatisticsDto>
{
	private readonly IStatisticsService _statisticsService;

	public GetStatisticsQueryHandler(IStatisticsService statisticsService)
	{
		_statisticsService = statisticsService;
	}

	public async Task<StatisticsDto> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
	{
		return await _statisticsService.GetCachedOrFetchStatisticsAsync(cancellationToken);
	}
}
