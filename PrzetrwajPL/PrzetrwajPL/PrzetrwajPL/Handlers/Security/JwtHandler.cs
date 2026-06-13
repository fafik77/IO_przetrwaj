namespace PrzetrwajPL.Handlers.Security;

public class JwtHandler : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		return await base.SendAsync(request, ct);
	}
}
