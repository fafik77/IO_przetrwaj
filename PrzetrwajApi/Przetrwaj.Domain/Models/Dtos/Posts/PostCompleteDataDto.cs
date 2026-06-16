using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos.Posts;

/// <summary>
/// Contains all the post data: full title & description, category, region, author.
/// As well as: votes count, users vote status (that can not be filled in from Post directly).
/// </summary>
public class PostCompleteDataDto
{
	public required string Id { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
	public CategoryDto? Category { get; set; }
	public CategoryType CategoryType { get; set; }
	public RegionOnlyDto? Region { get; set; }
	public LatLong? LatLong { get; set; }

	public UserGeneralDtoNoRegion? Author { get; set; }
	public DateTimeOffset DateCreated { get; set; }
	public VoteDto? MyVote { get; set; } = null;


	///To add all this bellow
	public long VotePositive { get; set; }
	public long VoteNegative { get; set; }

	public virtual IEnumerable<CommentDto?>? Comments { get; set; } = [];
	public virtual IEnumerable<AttachmentDto?> Attachments { get; set; } = [];


	public static PostCompleteDataDto? Map(Post? post, string baseUrl)
	{
		if (post is null) return null;
		return post is null ? null : new PostCompleteDataDto
		{
			Id = post.IdPost,
			Title = post.Title,
			Description = post.Description,
			//if CustomCategory, fill this data with {id=customId, Name=CustomName not "other/inne"}
			Category = CategoryDto.Map(post.IdCategoryNavigation),
			CategoryType = post.CategoryType,
			Region = RegionOnlyDto.Map(post.RegionNavigation),
			LatLong = post.Lat is null ? null : new LatLong((double)post.Lat, (double)post.Long!),
			Author = (UserGeneralDtoNoRegion?)post.IdAutorNavigation,
			DateCreated = post.DateCreated,
			Attachments = post.Attachments.Select(a => AttachmentDto.Map(a, baseUrl)).ToList(),
			Comments = post.Comments.Select(c => CommentDto.Map(c)).ToList(),
		};
	}
}
