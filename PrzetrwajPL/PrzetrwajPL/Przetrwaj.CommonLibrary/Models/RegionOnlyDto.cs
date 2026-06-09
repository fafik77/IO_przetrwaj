namespace Przetrwaj.CommonLibrary.Models;

public record RegionOnlyDto
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public int ParentId { get; set; }
}

public record RegionOnlyWithinDto : RegionOnlyDto
{
	public string? In { get; set; }
	public RegionPrecision Type { get; set; }
}