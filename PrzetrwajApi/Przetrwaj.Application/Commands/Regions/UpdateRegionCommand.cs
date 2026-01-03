using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Regions;

public class UpdateRegionCommand : ICommand
{
	[Required]
	[Key]
	public int IdRegion { get; set; }
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }
}
