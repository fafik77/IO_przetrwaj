using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions.RegionException;

public class RegionAlreadyExistsException : AlreadyExistsException<Region>
{
	public RegionAlreadyExistsException(string identity) : base(identity)
	{
	}
}
