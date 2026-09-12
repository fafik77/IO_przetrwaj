using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class PostNotValidException : BaseException
{
	public PostNotValidException(string msg) : base(msg)
	{ }

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
