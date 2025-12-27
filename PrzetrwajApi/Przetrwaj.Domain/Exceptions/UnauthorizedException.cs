using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class UnauthorizedException(string msg) : BaseException(msg)
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.Unauthorized;
}
