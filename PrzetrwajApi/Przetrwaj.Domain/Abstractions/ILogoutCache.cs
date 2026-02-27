namespace Przetrwaj.Domain.Abstractions;

public interface ILogoutCache
{
	void Logout(string userId, string TokenId);
	void Logout(string userId, IEnumerable<string> TokenIds);
	bool IsLogedOut(string userId, string TokenId);
}