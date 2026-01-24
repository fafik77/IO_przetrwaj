using System.Net.Http.Headers;

namespace PrzetrwajPL.Middleware;

public class JwtHandler : DelegatingHandler
{
	//private readonly ILocalStorageService _localStorage;
	//public JwtHandler(ILocalStorageService localStorage) => _localStorage = localStorage;

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		//var token = await _localStorage.GetItemAsync<string>("authToken");

		//if (!string.IsNullOrEmpty(token))
		//{
		//	request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		//}

		return await base.SendAsync(request, ct);
	}
}
