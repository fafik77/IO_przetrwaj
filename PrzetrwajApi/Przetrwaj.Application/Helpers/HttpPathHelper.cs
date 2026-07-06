using Microsoft.AspNetCore.Http;

namespace Przetrwaj.Application.Helpers;

public abstract class HttpPathHelper
{
	static public Uri HttpPath(IHttpContextAccessor httpContextAccessor) => HttpPath(httpContextAccessor.HttpContext?.Request);
	static public Uri HttpPath(HttpRequest httpRequest)
	{
		return new Uri($"{httpRequest.Scheme}://{httpRequest.Host}");
	}
}
