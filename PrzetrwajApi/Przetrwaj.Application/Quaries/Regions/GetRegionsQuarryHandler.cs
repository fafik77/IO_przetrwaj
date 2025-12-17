using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.RegionQauries;

public class GetRegionsQuarryHandler : IQueryHandler<GetRegionsQuarry, IEnumerable<RegionOnlyDto>>
{
	private readonly IRegionRepository _regionRepository;

	public GetRegionsQuarryHandler(IRegionRepository regionRepository)
	{
		_regionRepository = regionRepository;
	}


	public async Task<IEnumerable<RegionOnlyDto>> Handle(GetRegionsQuarry request, CancellationToken cancellationToken)
	{
		var res = await _regionRepository.GetAllAsync(cancellationToken);
		if (res == null) throw new InvalidOperationException("Invalid region quarry");
		return res
			.Select(r => (RegionOnlyDto)r)
			.ToList();
	}
}
