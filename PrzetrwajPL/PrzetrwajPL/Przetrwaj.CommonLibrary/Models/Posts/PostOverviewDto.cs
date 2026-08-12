using Przetrwaj.CommonLibrary.Requests;

namespace Przetrwaj.CommonLibrary.Models.Posts;


/// <summary>
/// Contains general data about the post. Usefull for listing a bunch of posts
/// </summary>
public class PostOverviewDto
{
	public required string Id { get; set; }
	public required string Title { get; set; }
	public CategoryDto? Category { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public LatLong? LatLong { get; set; }
	public DateTimeOffset DateCreated { get; set; }
	public UserGeneralDtoNoRegion? Author { get; set; }

	public long VotePositive { get; set; }
	public long VoteNegative { get; set; }
}
