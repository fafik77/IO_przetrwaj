using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Helpers;
using Przetrwaj.Domain.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Regions;

public class AddRegionCommand : ICommand<RegionOnlyDto>
{
	[Required]
	public int Id { get; set; }
	[Required]
	[StringLength(maximumLength: 100, MinimumLength = 3)]
	public required string Name { get; set; }
	public LatLong? LatLong { get; set; }

	public IRegionInfo Map() { return Map(this); }
	static public IRegionInfo Map(AddRegionCommand request)
	{
		var (Woj, Pow, Gmi) = RegionCompoundHelper.RegionSplit(request.Id);
		if (Gmi != 0)
			return new RegionGmi
			{
				Id = Gmi,
				Name = request.Name,
				PowId = Pow,
				Lat = request.LatLong!.Lat,
				Long = request.LatLong!.Long,
			};
		if (Pow != 0)
			return new RegionPow
			{
				Id = Pow,
				Name = request.Name,
				WojId = Woj,
				Lat = request.LatLong!.Lat,
				Long = request.LatLong!.Long,
			};
		return new RegionWoj
		{
			Id = Woj,
			Name = request.Name,
		};
	}
}
