using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class RegisterException(string msg) : BaseException(msg)
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
