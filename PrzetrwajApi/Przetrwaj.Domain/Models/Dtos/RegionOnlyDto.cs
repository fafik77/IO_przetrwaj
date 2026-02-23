using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class RegionOnlyDto
{
	public int Id { get; set; }
	/// Gmi?, Pow (, Woj)?
	public required string Name { get; set; }
	public string? In { get; set; }
	public LatLong? LatLong { get; set; }

	public static RegionOnlyDto? Map(IRegionInfo? region)
	{
		return region is null ? null : new RegionOnlyDto
		{
			Id = region.Id,
			Name = region.Name,
			LatLong = region.LatLong
		};
	}
}
