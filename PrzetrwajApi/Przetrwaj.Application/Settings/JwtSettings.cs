namespace Przetrwaj.Application.Settings;

public record JwtSettings
{
	public string Key { get; set; }
	public string Issuer { get; set; }
	public string Audience { get; set; }
	public double RefreshTokenValidHours { get; set; }
	public double ValidHours { get; set; }
}
