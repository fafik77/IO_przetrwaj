namespace Przetrwaj.CommonLibrary.Models;

public class CommentDto
{
    public required string CommentId { get; set; }
    public required string AuthorId { get; set; }
    public required string Comment { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public UserGeneralDtoNoRegion? Author { get; set; }
}
