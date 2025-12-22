using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class AddRegionCommand
{
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public required string RegionName { get; set; }
}
