using Przetrwaj.Application.Configuration.Commands;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Regions;

public class UpdateRegionCommand : ICommand
{
	[Required]
	[Key]
	public int IdRegion { get; set; }
	[Required]
	[MaxLength(100)]
	public required string Name { get; set; }
}
