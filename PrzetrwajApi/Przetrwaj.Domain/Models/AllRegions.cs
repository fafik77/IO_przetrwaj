using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models;

public class AllRegions
{
	public List<RegionWoj> Woj { get; set; } = [];
	public List<RegionPow> Pow { get; set; } = [];
	public List<RegionGmi> Gmi { get; set; } = [];
	public List<IRegionInfo> CompundList { get; set; } = [];
}
