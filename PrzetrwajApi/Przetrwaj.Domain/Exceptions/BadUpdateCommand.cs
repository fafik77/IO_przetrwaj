using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class BadUpdateCommand : BaseException
{
	public BadUpdateCommand(string msg) : base(msg)
	{	}

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
