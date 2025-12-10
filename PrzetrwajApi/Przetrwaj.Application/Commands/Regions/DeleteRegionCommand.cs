using Przetrwaj.Application.Configuration.Commands;

namespace Przetrwaj.Application.Commands.Regions;

public class DeleteRegionCommand : ICommand
{
	public int RegionId { get; set; }
}
