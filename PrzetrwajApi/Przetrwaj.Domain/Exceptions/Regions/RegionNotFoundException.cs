using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions.Regions;

public class RegionNotFoundException(int id) : NotFoundException<Entities.IRegionInfo>(id)
{ }
