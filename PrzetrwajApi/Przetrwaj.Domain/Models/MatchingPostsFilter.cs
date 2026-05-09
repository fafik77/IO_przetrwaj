using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models;

public record MatchingPostsFilter
{
	public int RegionId { get; set; }
	public int? Impediment { get; set; }
	public RegionPrecision? Level { get; set; }
	public CategoryTypeFilter CategoryFilter { get; set; } = CategoryTypeFilter.Both;
}
