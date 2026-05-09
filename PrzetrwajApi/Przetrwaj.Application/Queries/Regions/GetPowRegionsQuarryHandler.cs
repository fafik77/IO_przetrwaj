using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Regions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Regions;

public class GetPowRegionsQuarryHandler : IQueryHandler<GetPowRegionsQuarry, IEnumerable<RegionOnlyDto>>
{
	private readonly IRegionRepository _regionRepository;

	public GetPowRegionsQuarryHandler(IRegionRepository regionRepository)
	{
		_regionRepository = regionRepository;
	}

	public async Task<IEnumerable<RegionOnlyDto>> Handle(GetPowRegionsQuarry request, CancellationToken cancellationToken)
	{
		var res = await _regionRepository.GetAllAsync(cancellationToken);
		return res.Pow
			.Select(r => RegionOnlyDto.Map(r)!)
			.ToList();
	}
}
