using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.RegionCommands;

public class AddRegionCommand : ICommand<RegionOnlyDto>
{
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public required string RegionName { get; set; }
}
