using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace PrzetrwajPL.Handlers.Security;

public class JwtAuthorizationHandler : DelegatingHandler
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public JwtAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var context = _httpContextAccessor.HttpContext;

		var token = await context.GetTokenAsync("access_token");
		if (!string.IsNullOrEmpty(token))
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		return await base.SendAsync(request, cancellationToken);
	}
}