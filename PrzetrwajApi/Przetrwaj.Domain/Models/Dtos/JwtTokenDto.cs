namespace Przetrwaj.Domain.Models.Dtos;

public record JwtTokenDto
{
	public string? Token { get; set; }
	public string? RefreshToken { get; set; }
	public bool Success { get; set; } = true;
}
