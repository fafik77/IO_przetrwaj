namespace Przetrwaj.Domain.Models;

public record TokenRequest
{
	public required string AccessToken { get; set; }
	public required string RefreshToken { get; set; }
}
