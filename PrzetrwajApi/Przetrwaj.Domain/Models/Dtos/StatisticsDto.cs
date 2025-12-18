namespace Przetrwaj.Domain.Models.Dtos;

public class StatisticsDto
{
	public long Regions { get; set; }
	public long Users { get; set; }
	public long ActiveDangers { get; set; }
	public long ActiveResources { get; set; }
	public long Moderators { get; set; }
}
