using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class NotTheAuthorException : BaseException
{
	public NotTheAuthorException(string msg) : base(msg)
	{
	}

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
