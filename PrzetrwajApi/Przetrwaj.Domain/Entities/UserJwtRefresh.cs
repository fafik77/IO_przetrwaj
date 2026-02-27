using System.Text.Json.Serialization;

namespace Przetrwaj.Domain.Entities;

public class UserJwtRefresh
{
	[JsonPropertyName("UID")]
	public required string UserId { get; set; }
	public required string Jwi { get; set; }


	[JsonPropertyName("RT")]
	public required string RefreshToken { get; set; }
	[JsonPropertyName("VT")]
	public required DateTimeOffset ValidTill { get; set; }
	[JsonPropertyName("UL")]
	public short UsesLeft { get; set; }
}
