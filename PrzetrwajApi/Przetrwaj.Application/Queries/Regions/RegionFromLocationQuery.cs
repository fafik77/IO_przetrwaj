using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Queries.Regions;

public class RegionFromLocationQuery : IQuery<RegionOnlyDto?>
{
	public required LatLong location;
}
