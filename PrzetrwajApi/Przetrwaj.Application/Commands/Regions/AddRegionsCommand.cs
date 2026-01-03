using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Regions;

public class AddRegionsCommand : ICommand<IEnumerable<RegionOnlyDto>>
{
	[Required]
	public required IEnumerable<AddRegionCommand> Regions { get; set; }

	static public implicit operator List<Region>(AddRegionsCommand request)
	{
		return request.Regions.Select(r => (Region)r).ToList();
	}
}
