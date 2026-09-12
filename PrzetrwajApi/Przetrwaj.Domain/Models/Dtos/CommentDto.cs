using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public record CommentDto
{
    public required string CommentId { get; set; }
    public required string Comment { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public UserGeneralDtoNoRegion? Author { get; set; }


    public static CommentDto Map(UserComment comment)
    {
        return new CommentDto
        {
            CommentId = comment.IdComment,
            Comment = comment.Comment,
            DateCreated = comment.DateCreated,
            Author = UserGeneralDtoNoRegion.Map(comment.IdAutorNavigation),
        };
    }
}
