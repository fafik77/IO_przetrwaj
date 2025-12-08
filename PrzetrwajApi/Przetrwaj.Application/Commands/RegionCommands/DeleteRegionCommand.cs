using Przetrwaj.Application.Configuration.Commands;

namespace Przetrwaj.Application.Commands.RegionCommands;

public class DeleteRegionCommand : ICommand
{
	public int RegionId { get; set; }
}
