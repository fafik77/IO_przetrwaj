using Przetrwaj.Domain.Entities;
using System.Collections.Frozen;

namespace Przetrwaj.Domain.Models;

public class AllRegions
{
	public List<RegionWoj> Woj { get; set; } = [];
	public List<RegionPow> Pow { get; set; } = [];
	public List<RegionGmi> Gmi { get; set; } = [];
	public FrozenDictionary<int, IRegionInfo> CompundDict { get; set; } = FrozenDictionary<int, IRegionInfo>.Empty;
}
