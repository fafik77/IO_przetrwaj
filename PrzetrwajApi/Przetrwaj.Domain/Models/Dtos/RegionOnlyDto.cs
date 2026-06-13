using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public record RegionOnlyDto
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public int ParentId { get; set; }

	public static RegionOnlyDto? Map(IRegionInfo? region)
	{
		return region is null ? null : new RegionOnlyDto
		{
			Id = region.Id,
			Name = region.Name,
			ParentId = region.ParentId ?? 0
		};
	}
}

public record RegionOnlyWithinDto : RegionOnlyDto
{
	public string? In { get; set; }
	public RegionPrecision Type { get; set; }

	public static RegionOnlyWithinDto? Map(IRegionInfo? region)
	{
		return region is null ? null : new RegionOnlyWithinDto
		{
			Id = region.Id,
			Name = region.Name,
			Type = region.Type,
			ParentId = region.ParentId ?? 0
		};
	}
}
