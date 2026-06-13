namespace Przetrwaj.Domain.Abstractions;

public interface ILogoutCache
{
	void Logout(string userId, string TokenId);
	bool IsLogedOut(string userId, string TokenId);
}