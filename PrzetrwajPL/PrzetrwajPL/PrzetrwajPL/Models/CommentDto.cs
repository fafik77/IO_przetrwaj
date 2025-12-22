namespace PrzetrwajPL.Models;

public class CommentDto
{
	//there is no point in including Id, or Post info (as we already know the post)
	public required string Comment { get; set; }
	public DateTimeOffset DateCreated { get; set; }
	public UserGeneralDto? Autor { get; set; }
}
