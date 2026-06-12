namespace Przetrwaj.CommonLibrary.Consts;

public static class UserRoles
{
	public const string User = "User";
	public const string Moderator = "Moderator";
	public const string Admin = "Admin";
}

public static class ClaimNames
{
	/// <summary>
	/// RegionGmi stores id as string
	/// </summary>
	public const string Region = "Region";
	public const string Surname = "Surname";
	public const string Name = "Name";
	/// <summary>
	/// user.Impediments stores int as string. decomposes to bit field
	/// </summary>
	public const string Impediments = "Type";
}

public static class Consts
{
	public const string PrzetrwajApiClientName = "ServerAPI";
	public static readonly StringComparer PolishAlphabetComparer =
		StringComparer.Create(new System.Globalization.CultureInfo("pl-PL"), ignoreCase: true);
	public const string PrzetrwajAuthCookie = "PrzetrwajAuthCookie";
	public const string RedirectLoggedInUserTo = "/list";
}
