using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions.RegionException;

public class RegionNotFoundException : NotFoundException<Entities.Region>
{
	public RegionNotFoundException(int id) : base(id)
	{
	}
}
