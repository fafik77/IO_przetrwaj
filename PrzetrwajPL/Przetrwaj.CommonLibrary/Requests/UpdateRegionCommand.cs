using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class UpdateRegionCommand
{
	[Required]
	public int IdRegion { get; set; }
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }
}
