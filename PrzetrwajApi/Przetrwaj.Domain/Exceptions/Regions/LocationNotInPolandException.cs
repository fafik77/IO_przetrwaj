using Przetrwaj.Domain.Exceptions._base;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Exceptions.Regions;

public class LocationNotInPolandException(LatLong location) : NotFoundException<Entities.IRegionInfo>(location.ToString())
{ }
