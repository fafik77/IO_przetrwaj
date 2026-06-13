using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Queries.Regions;

public class RegionFromLocationQueryHandler : IQueryHandler<RegionFromLocationQuery, RegionOnlyDto?>
{
	private readonly IRegionRepository _regionRepository;

	public RegionFromLocationQueryHandler(IRegionRepository regionRepository)
	{
		_regionRepository = regionRepository;
	}

	public async Task<RegionOnlyDto?> Handle(RegionFromLocationQuery request, CancellationToken cancellationToken)
	{
		return RegionOnlyDto.Map(await _regionRepository.RegionFromLocationAsync(request.location, cancellationToken));
	}
}
