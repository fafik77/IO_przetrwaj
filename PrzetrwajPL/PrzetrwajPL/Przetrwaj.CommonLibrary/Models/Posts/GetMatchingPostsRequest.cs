namespace Przetrwaj.CommonLibrary.Models.Posts;

public class GetMatchingPostsRequest
{
	public int RegionId { get; set; }
	public int? Impediment { get; set; }
	public RegionPrecision? MaxLevel { get; set; }
	public CategoryTypeFilter? Category { get; set; }

	/// <summary>
	/// Converts the request properties into a null-safe dictionary for query strings.
	/// </summary>
	public Dictionary<string, string?> ToQueryDictionary() => new()
	{
		[nameof(RegionId)] = RegionId.ToString(),
		// If null, these evaluate to null, allowing QueryHelpers to skip them completely
		[nameof(Impediment)] = Impediment?.ToString(),
		[nameof(MaxLevel)] = MaxLevel.HasValue
				? (MaxLevel.Value).ToString()
				: null,
		[nameof(Category)] = Category.HasValue
				? (Category.Value).ToString()
				: null
	};
}
