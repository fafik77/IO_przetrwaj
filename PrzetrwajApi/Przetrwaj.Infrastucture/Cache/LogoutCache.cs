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
		var BlackListTimeSpanHour = double.Parse(CacheSettings["BlackListTimeSpanHour"]);
		_banCacheDuration = TimeSpan.FromHours(BlackListTimeSpanHour);
	}
	public class UserJwis
	{
		public HashSet<string> Keys { get; set; } = [];
	}

	public bool IsLogedOut(string userId, string TokenId)
	{
		_cache.TryGetValue(userId, out UserJwis? Tokens);
		if (Tokens is null) return false;
		return Tokens.Keys.Contains(TokenId);
	}

	public void Logout(string userId, string TokenId)
	{
		_cache.TryGetValue(userId, out UserJwis? UserJwis);
		//var List = new List<string>(TokenIds);
		if (UserJwis == null) UserJwis = new UserJwis();
		//List.AddRange(UserJwis.Keys);
		UserJwis.Keys.Add(TokenId);
		var options = new MemoryCacheEntryOptions
		{
			Size = UserJwis.Keys.Count,
			AbsoluteExpirationRelativeToNow = _banCacheDuration,
		};
		_cache.Set(userId, UserJwis, options);

	}
}
