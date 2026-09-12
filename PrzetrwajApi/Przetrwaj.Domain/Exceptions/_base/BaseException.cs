using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public abstract class BaseException : Exception
{
	public abstract HttpStatusCode HttpStatusCode { get; }
	public BaseException(string msg) : base(msg) { }
}
