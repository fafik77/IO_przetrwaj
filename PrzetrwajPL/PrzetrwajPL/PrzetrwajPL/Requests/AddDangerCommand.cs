using System.ComponentModel.DataAnnotations;

namespace PrzetrwajPL.Requests;

public class AddDangerCommand
{
	[Required]
	[MaxLength(200)]
	[MinLength(3)]
	public required string Title { get; set; }
	[MaxLength(2000)]
	public string? Description { get; set; }
	public int IdCategory { get; set; }
	[MaxLength(100)]
	public string? CustomCategory { get; set; }
	public int IdRegion { get; set; }
}

