using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Exceptions;

public class LocationNotInPolandException(LatLong location) : NotFoundException<Entities.IRegionInfo>(location.ToString())
{ }
