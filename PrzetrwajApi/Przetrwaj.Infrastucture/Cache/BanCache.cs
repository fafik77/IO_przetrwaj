using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Infrastucture.Cache;

public class BanCache : IBanCache
{
	private readonly IMemoryCache _cache;
	private readonly TimeSpan _banCacheDuration = TimeSpan.FromHours(8); //cookies are set for 8 hours so this one also for 8h (login is only valid for 30min by Default)
	private readonly IServiceProvider _services;
	private readonly IConfiguration _configuration;

	public BanCache(IMemoryCache cache, IServiceProvider services, IConfiguration configuration)
	{
		_cache = cache;
		_services = services;
		var CacheSettings = configuration.GetSection("Cache");
		var BlackListTimeSpanHour = int.Parse(CacheSettings["BlackListTimeSpanHour"]);
		_banCacheDuration = TimeSpan.FromHours(BlackListTimeSpanHour);
		_configuration = configuration;
	}

	public void BanUser(string userId)
	{
		_cache.Set(userId, new BanStatus { Banned = true }, _banCacheDuration);
	}

	public async Task<BanInfo> GetUserBanInfoAsync(string userId)
	{
		if (_cache.TryGetValue(userId, out BanStatus? banStatus) && banStatus?.BanInfo != null)
		{
			return banStatus.BanInfo;
		}
		else
		{
			using var scope = _services.CreateScope();
			var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
			var bannedUser = await _userRepository.GetByIdAsync(userId);
			if (bannedUser is null) throw new UserNotFoundException(userId);
			var moderatorUser = await _userRepository.GetByIdAsync(bannedUser.BannedById ?? string.Empty);

			banStatus = _cache.Set(userId, new BanStatus { Banned = true }, _banCacheDuration);
			banStatus.BanInfo = new BanInfo
			{
				BanDate = bannedUser.BanDate,
				BanReason = bannedUser.BanReason ?? string.Empty,
				BannedById = bannedUser.BannedById ?? string.Empty,
				Banned = bannedUser.BanDate != null,
				BannedBy = UserGeneralDto.Map(moderatorUser)
			};
			return banStatus.BanInfo;
		}
	}

	public bool IsUserBanned(string userId)
	{
		return _cache.TryGetValue(userId, out BanStatus? banStatus) && banStatus != null && banStatus.Banned;
	}
}
