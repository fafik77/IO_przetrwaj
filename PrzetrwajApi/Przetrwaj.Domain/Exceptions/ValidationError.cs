using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class ValidationError(string msg) : BaseException(msg)
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
