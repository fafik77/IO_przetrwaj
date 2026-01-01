using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos.Posts;

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
	public long VotePositive { get; set; }
	public long VoteNegative { get; set; }
	//public long VoteSum { get; set; }
	//public float VoteRatio { get; set; }

	public virtual IEnumerable<CommentDto?>? Comments { get; set; } = [];
	public virtual IEnumerable<AttachmentDto?> Attachments { get; set; } = [];



	public static explicit operator PostCompleteDataDto?(Post? post)
	{
		if (post is null) return null;
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
			//Attachments = post.Attachments.Select(a => (AttachmentDto?)a).ToList(),
			Comments = post.Comments.Select(c => (CommentDto)c).ToList(),
		};
	}
}
