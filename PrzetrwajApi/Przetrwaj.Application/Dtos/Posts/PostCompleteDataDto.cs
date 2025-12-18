using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Dtos.Posts;

/// <summary>
/// Contains all the post data: full title & description, category, region, author, 
/// </summary>
public class PostCompleteDataDto
{
	public required string Id { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
	public CategoryDto? Category { get; set; }
	public CategoryType CategoryType { get; set; }
	public RegionOnlyDto? Region { get; set; }

	public UserGeneralDto? Author { get; set; }
	public DateTimeOffset DateCreated { get; set; }


	///To add all this bellow
	public int VotePositive { get; set; }
	public int VoteNegative { get; set; }
	public int VoteSum { get; set; }
	public int VoteRatio { get; set; }

	//public int CommentsAmount { get; set; }

	public virtual IEnumerable<CommentDto>? Comments { get; set; } = new List<CommentDto>();
	public virtual IEnumerable<AttachmentDto>? Attachments { get; set; }// = new List<Attachment>();



	public static explicit operator PostCompleteDataDto?(Post? post)
	{
		int positive, negative, sum, ratio;
		return post is null ? null : new PostCompleteDataDto
		{
			Id = post.IdPost,
			Title = post.Title,
			Description = post.Description,
			//if CustomCategory, fill this data with {id=customId, Name=CustomName not "other/inne"}
			Category = (CategoryDto?)post.IdCategoryNavigation,
			CategoryType = post.Category,
			Region = (RegionOnlyDto?)post.IdRegionNavigation,
			Author = (UserGeneralDto?)post.IdAutorNavigation,
			DateCreated = post.DateCreated,
		};
	}
}
