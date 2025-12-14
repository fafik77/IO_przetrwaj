using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Quaries.RegionQauries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.RegionException;

namespace Przetrwaj.Application.Quaries.Regions;

public class GetRegionQuarryHandler : IQueryHandler<GetRegionQuarry, RegionOnlyDto>
{
	private readonly IRegionRepository _regionRepository;

	public GetRegionQuarryHandler(IRegionRepository regionRepository)
	{
		_regionRepository = regionRepository;
	}

	public async Task<RegionOnlyDto> Handle(GetRegionQuarry request, CancellationToken cancellationToken)
	{
		var res = await _regionRepository.GetByIdAsync(request.IdRegion, cancellationToken);
		if (res is null) throw new RegionNotFoundException(request.IdRegion);
		return (RegionOnlyDto)res;
	}
}
