namespace Przetrwaj.Domain.Models;

public record LoginWithExternalProvider : RegisterEmailInfo
{
    /// <summary>
    /// Name of LoginProvider eg. Google.
    /// </summary>
    public required string LoginProvider { get; set; }

    public required string ProviderKey { get; set; }
}
