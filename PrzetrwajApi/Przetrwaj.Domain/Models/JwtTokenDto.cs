namespace Przetrwaj.Domain.Models;

public class JwtTokenDto
{
	public string? Token { get; set; }
	public bool Success { get; set; } = true;
}
