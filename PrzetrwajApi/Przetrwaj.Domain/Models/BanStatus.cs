namespace Przetrwaj.Domain.Models;

public class BanStatus
{
	public bool Banned { get; set; } = false;
	public BanInfo? BanInfo { get; set; }
}
