using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class RegionOnlyDto
{
	public int IdRegion { get; set; }
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }


	public static explicit operator RegionOnlyDto?(Region region)
	{
		return region is null ? null : new RegionOnlyDto
		{
			IdRegion = region.IdRegion,
			Name = region.Name,
			Lat = region.Lat,
			Long = region.Long,
		};
	}
}
