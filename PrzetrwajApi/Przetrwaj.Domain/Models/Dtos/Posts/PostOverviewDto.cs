using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

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
	public int VotePositive { get; set; }
	public int VoteNegative { get; set; }
	public int VoteSum { get; set; }
	public int VoteRatio { get; set; }

	public int CommentsAmount { get; set; }



	public static explicit operator PostOverviewDto?(Post? post)
	{
		int positive, negative, sum, ratio;
		return post is null ? null : new PostOverviewDto
		{
			Id = post.IdPost,
			Title = post.Title,
			Category = (CategoryDto?)post.IdCategoryNavigation,
			Region = (RegionOnlyDto?)post.IdRegionNavigation,
			DateCreated = post.DateCreated,

			//VoteSum
		};
	}
}
