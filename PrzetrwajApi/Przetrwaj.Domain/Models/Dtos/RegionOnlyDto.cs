using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class RegionOnlyDto
{
	public int Id { get; set; }
	/// Gmi?, Pow (, Woj)?
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }


	public static explicit operator RegionOnlyDto?(Region region)
	{
		return region is null ? null : new RegionOnlyDto
		{
			Id = region.IdRegion,
			Name = region.Name,
			Lat = region.Lat,
			Long = region.Long,
		};
	}
}
