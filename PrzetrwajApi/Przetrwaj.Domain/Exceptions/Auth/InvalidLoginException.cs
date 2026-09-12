using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class InvalidLoginException(string message) : BaseException(message)
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
