using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class LatLong(double Lat, double Long)
{
	public double Lat { get; set; } = Lat;
	public double Long { get; set; } = Long;
}

public class RegionDto
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public LatLong? LatLong { get; set; }


	public static RegionDto? Map(IRegionInfo region)
	{
		return region is null ? null : new RegionDto
		{
			Id = region.Id,
			Name = region.Name,
			LatLong = region.LatLong,
		};
	}
}