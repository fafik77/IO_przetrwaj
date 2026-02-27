using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Infrastucture.Cache;

public class LogoutCache : ILogoutCache
{
	private readonly IMemoryCache _cache;
	private readonly TimeSpan _banCacheDuration = TimeSpan.FromHours(8); //cookies are set for 8 hours so this one also for 8h (login is only valid for 30min by Default)
	private readonly IServiceProvider _services;
	private readonly IConfiguration _configuration;

	public LogoutCache(IMemoryCache cache, IServiceProvider services, IConfiguration configuration)
	{
		_cache = cache;
		_services = services;
		_configuration = configuration;
		var CacheSettings = configuration.GetSection("Cache");
		var BlackListTimeSpanHour = int.Parse(CacheSettings["BlackListTimeSpanHour"]);
		_banCacheDuration = TimeSpan.FromHours(BlackListTimeSpanHour);
	}

	public bool IsLogedOut(string userId, string TokenId)
	{
		_cache.TryGetValue(userId, out ISet<string>? Tokens);
		if (Tokens is null) return false;
		return Tokens.Contains(TokenId);
	}

	public void Logout(string userId, string TokenId)
	{
		Logout(userId, [TokenId]);
	}

	public void Logout(string userId, IEnumerable<string> TokenIds)
	{
		_cache.TryGetValue(userId, out IEnumerable<string>? Tokens);
		var List = new List<string>(TokenIds);
		if (Tokens != null)
			List.AddRange(Tokens);
		_cache.Set(userId, List.ToHashSet(), _banCacheDuration);
	}
}
