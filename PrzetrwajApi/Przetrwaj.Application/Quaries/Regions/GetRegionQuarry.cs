using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.RegionQauries;

public class GetRegionQuarry : IQuery<RegionOnlyDto>
{
	public int IdRegion;
}
