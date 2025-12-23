using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions;

public class InvalidImageException(string fileName) : BaseException(($@"""{fileName}"" is not an image"))
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.NotAcceptable;
}
