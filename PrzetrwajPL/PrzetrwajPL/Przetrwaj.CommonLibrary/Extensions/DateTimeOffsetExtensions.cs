using System;

namespace Przetrwaj.CommonLibrary.Extensions;

public static class DateTimeOffsetExtensions
{
	private static readonly TimeZoneInfo SingaporeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
	private static readonly TimeZoneInfo WarsawZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

	/// <summary>
	/// Converts a DateTimeOffset to Singapore Standard Time (GMT+8), respecting any historical or current rules.
	/// </summary>
	public static DateTimeOffset ToSingaporeTime(this DateTimeOffset dateTimeOffset)
	{
		return TimeZoneInfo.ConvertTime(dateTimeOffset, SingaporeZone);
	}

	/// <summary>
	/// Converts a DateTimeOffset to Central European Time (GMT+1/GMT+2 for Warsaw, Poland), respecting daylight saving time rules.
	/// </summary>
	public static DateTimeOffset ToWarsawTime(this DateTimeOffset dateTimeOffset)
	{
		return TimeZoneInfo.ConvertTime(dateTimeOffset, WarsawZone);
	}
}
