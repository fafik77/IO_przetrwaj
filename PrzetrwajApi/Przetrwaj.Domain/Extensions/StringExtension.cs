namespace Przetrwaj.Domain.Extensions;

public static class StringExtension
{
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);
}
