using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions.Auth;

public class InvalidAuthorizationException(string message) : BaseException(message)
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
