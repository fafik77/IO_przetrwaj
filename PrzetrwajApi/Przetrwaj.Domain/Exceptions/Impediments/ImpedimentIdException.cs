using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions.Impediments;

public class ImpedimentIdException(string msg) : BaseException(msg)
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
