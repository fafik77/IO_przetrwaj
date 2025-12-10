using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Dtos;

public class PostDto
{
	public required string Id { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
	public CategoryDto? Category { get; set; }
	public RegionOnlyDto? Region { get; set; }

	public UserGeneralDto? Author { get; set; }
	public DateTime DateCreated { get; set; }


	///To add all this bellow
	public int VotePositive { get; set; }
	public int VoteNegative { get; set; }
	public int VoteSum { get; set; }
	public int VoteRatio { get; set; }

	public int CommentsAmount { get; set; }

	//public virtual IEnumerable<Attachment>? Attachments { get; set; }// = new List<Attachment>();



	public static explicit operator PostDto?(Post? post)
	{
		int positive, negative, sum, ratio;
		return post is null ? null : new PostDto
		{
			Id = post.IdPost,
			Title = post.Title,
			Description = post.Description,
			Category = (CategoryDto?)post.IdCategoryNavigation,
			Region = (RegionOnlyDto?)post.IdRegionNavigation,
			Author = (UserGeneralDto?)post.IdAutorNavigation,
			DateCreated = post.DateCreated,

			//VoteSum
		};
	}
}
