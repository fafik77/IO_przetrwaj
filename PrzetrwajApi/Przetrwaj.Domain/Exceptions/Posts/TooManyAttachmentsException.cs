using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class TooManyAttachmentsException : BaseException
{
	public TooManyAttachmentsException(string msg) : base(msg)
	{
	}

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
