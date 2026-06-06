namespace Przetrwaj.CommonLibrary.Consts;

public static class Consts
{
	public static readonly string PrzetrwajApiClientName = "ServerAPI";
	public static readonly StringComparer PolishAlphabetComparer =
		StringComparer.Create(new System.Globalization.CultureInfo("pl-PL"), ignoreCase: true);
}
