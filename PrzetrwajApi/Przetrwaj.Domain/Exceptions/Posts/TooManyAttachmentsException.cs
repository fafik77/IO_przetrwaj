using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions.Posts;

public class TooManyAttachmentsException : BaseException
{
	public TooManyAttachmentsException(string msg) : base(msg)
	{
	}

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
