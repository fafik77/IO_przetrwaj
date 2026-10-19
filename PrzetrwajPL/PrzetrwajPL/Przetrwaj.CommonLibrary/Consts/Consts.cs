namespace Przetrwaj.CommonLibrary.Consts;

/// <summary>
/// Name of user roles in JWT.
/// </summary>
public static class UserRoles
{
    public const string User = "User";
    public const string Moderator = "Moderator";
    public const string Admin = "Admin";
}

/// <summary>
/// Names of claims in JWT of user.
/// </summary>
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
    /// <summary>
    /// Name of the Http Client that connects to Przetrwaj Api.
    /// </summary>
    public const string PrzetrwajApiClientName = "ServerAPI";

    /// <summary>
    /// Name of the Http Client that that refreshes JWT tokens on Przetrwaj Api.
    /// </summary>
    public const string PrzetrwajApiRefreshClientName = "ServerAPIRefreshToken";

    /// <summary>
    /// Polish string comparer.
    /// </summary>
    public static readonly StringComparer PolishAlphabetComparer =
        StringComparer.Create(new System.Globalization.CultureInfo("pl-PL"), ignoreCase: true);

    /// <summary>
    /// Authorization store Cookie name.
    /// </summary>
    public const string PrzetrwajAuthCookie = "PrzetrwajAuthCookie";

    /// <summary>
    /// After login user is redirected to this page.
    /// </summary>
    public const string RedirectLoggedInUserTo = "/list";
}


public static class ApiPaths
{
    public static string PrzetrwajApiExternalUri { get; set; }
}
