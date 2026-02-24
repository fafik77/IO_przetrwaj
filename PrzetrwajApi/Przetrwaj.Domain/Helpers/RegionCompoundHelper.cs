namespace Przetrwaj.Domain.Helpers;

public class RegionCompoundHelper
{
	public static (short Woj, short Pow, int Gmi) RegionSplit(int CompoundRegionId)
	{
		(short Woj, short Pow, int Gmi) Region = new();
		if (CompoundRegionId < 10000)
		{
			Region.Pow = (short)CompoundRegionId;
			Region.Woj = (short)(CompoundRegionId / 100);
		}
		if (CompoundRegionId < 100) Region.Woj = (short)CompoundRegionId;

		if (CompoundRegionId > 10000)
		{
			Region.Gmi = CompoundRegionId;
			Region.Pow = (short)(CompoundRegionId / 1000);
			Region.Woj = (short)(CompoundRegionId / 100000);
		}
		return Region;
	}
	public static int UnifyRegionId(int RegionId)
	{
		if (RegionId < 100) return RegionId * 100_000;
		if (RegionId < 10000) return RegionId * 1_000;
		if (RegionId < 100000) return RegionId * 10;
		return RegionId;
	}
}
