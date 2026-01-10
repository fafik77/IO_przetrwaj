using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Regions;

public class AddRegionCommand : ICommand<RegionOnlyDto>
{
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public required string Name { get; set; }
	public double Lat { get; set; }
	public double Long { get; set; }

	static public implicit operator Region(AddRegionCommand request)
	{
		return new Region
		{
			Name = request.Name,
			Lat = request.Lat,
			Long = request.Long
		};
	}
}
