namespace Przetrwaj.CommonLibrary.Models;

public record StatisticsDto
{
	public int RegionsWoj { get; set; }
	public int RegionsPow { get; set; }
	public int RegionsGmi { get; set; }
	public int DangerCategories { get; set; }
	public int ResourceCategories { get; set; }

	public long Users { get; set; }
	public long ActiveDangers { get; set; }
	public long ActiveResources { get; set; }
	public long Moderators { get; set; }
}
