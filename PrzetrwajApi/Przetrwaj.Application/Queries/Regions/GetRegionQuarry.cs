using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Regions;

public class GetRegionQuarry : IQuery<RegionOnlyWithinDto>
{
	public int IdRegion;
}
