namespace PrzetrwajPL.Models.Posts;


/// <summary>
/// Contains general data about the post. Usefull for listing a bunch of posts
/// </summary>
public class PostOverviewDto
{
	public required string Id { get; set; }
	public required string Title { get; set; }
	public CategoryDto? Category { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public DateTimeOffset DateCreated { get; set; }


	///To add all this bellow
	public long VotePositive { get; set; }
	public long VoteNegative { get; set; }
	public long VoteSum { get; set; }
	public float VoteRatio { get; set; }
}
