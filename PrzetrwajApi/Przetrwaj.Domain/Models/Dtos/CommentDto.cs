using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public record CommentDto
{
	//there is no point in including Id, or Post info (as we already know the post)
	public required string Comment { get; set; }
	public DateTimeOffset DateCreated { get; set; }
	public UserGeneralDtoNoRegion? Autor { get; set; }


	public static CommentDto Map(UserComment comment)
	{
		return new CommentDto
		{
			Comment = comment.Comment,
			DateCreated = comment.DateCreated,
			Autor = (UserGeneralDtoNoRegion?)comment.IdAutorNavigation,
		};
	}
}
