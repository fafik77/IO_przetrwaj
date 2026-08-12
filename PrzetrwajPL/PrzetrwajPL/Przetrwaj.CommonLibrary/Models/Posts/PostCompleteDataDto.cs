using Przetrwaj.CommonLibrary.Requests;

namespace Przetrwaj.CommonLibrary.Models.Posts;

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
	public LatLong? LatLong { get; set; }

	public UserGeneralDtoNoRegion? Author { get; set; }
	public DateTimeOffset DateCreated { get; set; }
	public VoteDto? MyVote { get; set; }


	public long VotePositive { get; set; }
	public long VoteNegative { get; set; }

	public virtual IEnumerable<CommentDto>? Comments { get; set; } = [];
	public virtual IEnumerable<AttachmentDto> Attachments { get; set; } = [];
}
