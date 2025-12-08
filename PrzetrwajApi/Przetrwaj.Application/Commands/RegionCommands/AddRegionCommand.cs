using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;

namespace Przetrwaj.Application.Commands.RegionCommands;

public class AddRegionCommand : ICommand<RegionOnlyDto>
{
	public required string RegionName { get; set; }
}
