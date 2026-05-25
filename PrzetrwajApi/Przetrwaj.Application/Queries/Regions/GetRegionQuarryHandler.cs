using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Regions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Regions;

public class GetRegionQuarryHandler : IQueryHandler<GetRegionQuarry, RegionOnlyWithinDto>
{
	private readonly IRegionRepository _regionRepository;

	public GetRegionQuarryHandler(IRegionRepository regionRepository)
	{
		_regionRepository = regionRepository;
	}

	public async Task<RegionOnlyWithinDto> Handle(GetRegionQuarry request, CancellationToken cancellationToken)
	{
		var res = await _regionRepository.GetByIdAsync(request.IdRegion, cancellationToken);
		if (res is null) throw new RegionNotFoundException(request.IdRegion);
		var dto = RegionOnlyWithinDto.Map(res)!;
		var parents = new List<string>();
		var parentId = res.ParentId;
		while (parentId != null)
		{
			var parent = await _regionRepository.GetByIdAsync(parentId.Value, cancellationToken);
			parents.Add(parent!.Name);
			parentId = parent.ParentId;
		}
		parents.Reverse();
		dto.In = string.Join(" - ", parents);
		return dto;
	}
}
