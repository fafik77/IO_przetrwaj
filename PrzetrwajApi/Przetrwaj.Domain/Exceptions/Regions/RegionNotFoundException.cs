namespace Przetrwaj.Domain.Exceptions;

public class RegionNotFoundException(int id) : NotFoundException<Entities.IRegionInfo>(id)
{ }
