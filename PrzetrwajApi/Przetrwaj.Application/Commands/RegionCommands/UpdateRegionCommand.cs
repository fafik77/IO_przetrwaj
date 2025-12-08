using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.RegionCommands;

public class UpdateRegionCommand : ICommand
{
	[Key]
	public int IdRegion { get; set; }
	[MaxLength(100)]
	public required string Name { get; set; }
}
