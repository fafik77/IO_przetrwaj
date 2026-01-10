using Microsoft.AspNetCore.WebUtilities;

namespace Przetrwaj.Application;

public static class UrlExtensions
{
	public static string ToQueryString(this object obj, string baseUrl)
	{
		var properties = obj.GetType().GetProperties()
			.Where(p => p.GetValue(obj) != null)
			.ToDictionary(
				p => p.Name,
				p => p.GetValue(obj)?.ToString()
			);
		return QueryHelpers.AddQueryString(baseUrl, properties!);
	}
}