namespace PrzetrwajPL.Models;

public class RegionOnlyDto
{
	public int Id{ get; set; }
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }
}
