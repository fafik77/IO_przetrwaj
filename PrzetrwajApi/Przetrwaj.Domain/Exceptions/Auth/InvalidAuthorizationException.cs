using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class InvalidAuthorizationException(string message) : BaseException(message)
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
