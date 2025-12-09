using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Dtos;

public class CommentDto
{
	//there is no point in including Id, or Post info (as we already know the post)
	public required string Comment { get; set; }
	public DateTime DateCreated { get; set; }
	public UserGeneralDto? Autor { get; set; }


	public static explicit operator CommentDto(UserComment comment)
	{
		return new CommentDto
		{
			Comment = comment.Comment,
			DateCreated = comment.DateCreated,
			Autor = comment.IdAutorNavigation == null ? null : (UserGeneralDto)comment.IdAutorNavigation,
		};
	}

}
