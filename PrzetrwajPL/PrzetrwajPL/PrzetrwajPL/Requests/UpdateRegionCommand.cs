using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class UpdateRegionCommand
{
	[Required]
	public int IdRegion { get; set; }
	[Required]
	[MaxLength(100)]
	public required string Name { get; set; }
}
