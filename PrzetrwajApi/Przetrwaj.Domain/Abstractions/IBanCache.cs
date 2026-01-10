using Przetrwaj.Domain.Models;

namespace Przetrwaj.Domain.Abstractions;

public interface IBanCache
{
	void BanUser(string userId);
	bool IsUserBanned(string userId);
	Task<BanInfo> GetUserBanInfoAsync(string userId);
}
