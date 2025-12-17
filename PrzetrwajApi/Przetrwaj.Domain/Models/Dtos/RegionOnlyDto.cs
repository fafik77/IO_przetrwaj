using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class RegionOnlyDto
{
	public int IdRegion { get; set; }
	public required string Name { get; set; }


	public static explicit operator RegionOnlyDto?(Region region)
	{
		return region is null ? null : new RegionOnlyDto
		{
			IdRegion = region.IdRegion,
			Name = region.Name,
		};
	}
}
