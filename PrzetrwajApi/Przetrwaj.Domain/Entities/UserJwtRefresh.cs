namespace Przetrwaj.Domain.Entities;

public class UserJwtRefresh
{
	public required string UserId { get; set; }
	public required string Jwi { get; set; }


	public required string RefreshToken { get; set; }
	public required DateTimeOffset ValidTill { get; set; }
	public short UsesLeft { get; set; }
}
