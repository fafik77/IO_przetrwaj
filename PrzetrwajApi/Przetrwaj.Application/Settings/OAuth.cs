using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Settings;

public class OAuth
{
	public OAuthProvider? Google {  get; set; }
}

public class OAuthProvider
{
	[Required]
	public required string ClientId { get; set; }
	[Required]
	public required string ClientSecret { get; set; }
}
