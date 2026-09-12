namespace Przetrwaj.CommonLibrary.Models;

public class CommentDto
{
    public required string CommentId { get; set; }
    public required string AuthorId { get; set; }
    public required string Comment { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public UserGeneralDtoNoRegion? Author { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsEditing { get; set; } = false;

    [System.Text.Json.Serialization.JsonIgnore]
    public string? InEditComment { get; set; }
}
