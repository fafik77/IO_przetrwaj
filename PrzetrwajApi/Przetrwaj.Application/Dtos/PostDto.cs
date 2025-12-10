using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Dtos;

public class PostDto
{
	public required string Id { get; set; }
	public string Title { get; set; } = null!;
	public string Description { get; set; } = null!;
	public CategoryDto? Category { get; set; }
	public RegionOnlyDto? Region { get; set; }

	public UserGeneralDto? Author { get; set; }
	public DateTime DateCreated { get; set; }
	public bool Active { get; set; }


	///To add all this bellow
	public int VotePositive { get; set; }
	public int VoteNegative { get; set; }
	public int VoteSum { get; set; }
	public int VoteRatio { get; set; }

	//public virtual IEnumerable<Attachment>? Attachments { get; set; }// = new List<Attachment>();



	public static explicit operator PostDto(Post post)
	{
		int positive, negative, sum, ratio;
		return new PostDto
		{
			Id = post.IdPost,
			Title = post.Title,
			Description = post.Description,
			Category = (CategoryDto?)post.IdCategoryNavigation ?? null,
			Region = (RegionOnlyDto?)post.IdRegionNavigation ?? null,
			Author = (UserGeneralDto?)post.IdAutorNavigation ?? null,
			DateCreated = post.DateCreated,
			Active = post.Active,

			//VoteSum
		};
	}
}
