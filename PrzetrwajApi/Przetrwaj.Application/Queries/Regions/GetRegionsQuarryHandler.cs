using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Helpers;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Queries.Regions;

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
		var regions = request.Precision switch
		{
			RegionPrecision.WOJ => res.Woj,
			RegionPrecision.POW => res.Pow,
			RegionPrecision.GMI => res.Gmi,
			_ => res.CompundDict.Select(r => r.Value),
		};
		var ParentId = request.ParentId;
		if (ParentId != null)
			ParentId = RegionCompoundHelper.DeepestRegionId(ParentId.Value);
		return regions
			.Where(r => String.IsNullOrEmpty(request.NameLike) || r.Name.Contains(request.NameLike, StringComparison.OrdinalIgnoreCase))
			.Where(r => ParentId == null || r.ParentId == ParentId)
			.Select(r => RegionOnlyDto.Map(r)!);
	}
}
