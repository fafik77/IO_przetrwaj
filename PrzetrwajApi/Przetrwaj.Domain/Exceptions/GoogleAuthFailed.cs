using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class GoogleAuthFailed : BaseException
{
	public GoogleAuthFailed(string msg) : base(msg)
	{ }

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
