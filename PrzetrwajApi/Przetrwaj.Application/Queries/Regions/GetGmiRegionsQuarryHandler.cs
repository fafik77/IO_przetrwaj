using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Regions;

public class GetGmiRegionsQuarryHandler : IQueryHandler<GetGmiRegionsQuery, IEnumerable<RegionOnlyDto>>
{
	private readonly IRegionRepository _regionRepository;

	public GetGmiRegionsQuarryHandler(IRegionRepository regionRepository)
	{
		_regionRepository = regionRepository;
	}

	public async Task<IEnumerable<RegionOnlyDto>> Handle(GetGmiRegionsQuery request, CancellationToken cancellationToken)
	{
		var res = await _regionRepository.GetAllAsync(cancellationToken);
		return res.Gmi
			.Select(r => RegionOnlyDto.Map(r)!)
			.ToList();
	}
}