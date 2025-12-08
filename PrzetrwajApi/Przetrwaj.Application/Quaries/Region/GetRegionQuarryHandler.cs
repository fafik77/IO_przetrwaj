using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Quaries.Region;

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
		if (res == null) throw new InvalidOperationException("Invalid region quarry");
		return (RegionOnlyDto)res;
	}
}
