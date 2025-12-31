using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos.Posts;


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
	//public long VoteBalance { get; set; }
	//public long VoteSum { get; set; }
	//public float VoteRatio { get; set; }


	public static explicit operator PostOverviewDto?(Post? post)
	{
		return post is null ? null : new PostOverviewDto
		{
			Id = post.IdPost,
			Title = post.Title,
			Category = (CategoryDto?)post.IdCategoryNavigation,
			Region = (RegionOnlyDto?)post.IdRegionNavigation,
			DateCreated = post.DateCreated,
		};
	}
}
