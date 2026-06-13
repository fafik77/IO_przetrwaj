namespace Przetrwaj.Domain.Helpers;

public class RegionCompoundHelper
{
	/// <summary>
	/// Splits given region into ww-[pp-[gg]].
	/// Thats how it's stored in the database.
	/// </summary>
	/// <param name="CompoundRegionId">(6|7),4,2 digit TERC region id</param>
	/// <returns>2, 4, 6 digit ids for Woj, Pow, Gmi</returns>
	public static (short Woj, short Pow, int Gmi) RegionSplit(int CompoundRegionId)
	{
		(short Woj, short Pow, int Gmi) Region = new();
		if (CompoundRegionId < 100) Region.Woj = (short)CompoundRegionId;
		else if (CompoundRegionId < 10000)
		{
			Region.Pow = (short)CompoundRegionId;
			Region.Woj = (short)(CompoundRegionId / 100);
		}
		else if (CompoundRegionId > 10000)
		{
			CompoundRegionId = UnifyRegionIdTo7Digits(CompoundRegionId); // make sure the input is 7 digit (not 6)
			Region.Gmi = (CompoundRegionId / 10);  //removes the last 1 digit from format (2-2-2-1)
			Region.Pow = (short)(CompoundRegionId / 1000);
			Region.Woj = (short)(CompoundRegionId / 100000);
		}
		return Region;
	}
	/// <summary>
	/// Unifies region id to format ww-pp-gg-0
	/// </summary>
	/// <param name="RegionId">TERC region id</param>
	/// <returns>7 digit region id ww-pp-gg-0</returns>
	private static int UnifyRegionIdTo7Digits(int RegionId)
	{
		if (RegionId < 100) return RegionId * 100_000;
		if (RegionId < 10000) return RegionId * 1_000;
		if (RegionId < 100000) return RegionId * 10;
		if (RegionId < 1000000) return RegionId * 10;
		return (RegionId / 10) * 10;    //removes the last 1 digit from format (2-2-2-1)
	}

	/// <summary>
	/// Unifies region id to format ww-[pp-[gg]]
	/// </summary>
	/// <param name="RegionId">TERC region id</param>
	/// <returns>2|4|6 deepest region id</returns>
	public static int UnifyRegionId(int RegionId)
	{
		if (RegionId < 100) return RegionId;
		//standard TERC code is in format 2-2-2-1 but we dropped the last 1 digit
		if (RegionId > 1_00_00_00) RegionId /= 10;
		if (RegionId % 1_00 == 0) RegionId /= 1_00;
		if (RegionId % 1_00 == 0) RegionId /= 1_00;
		return RegionId;
	}
}
