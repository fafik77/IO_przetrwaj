using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Regions;

public class AddRegionCommand : ICommand<RegionOnlyDto>
{
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public required string RegionName { get; set; }
}
