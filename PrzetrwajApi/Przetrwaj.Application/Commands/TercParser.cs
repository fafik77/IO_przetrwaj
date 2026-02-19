using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands;

public class TERCItem
{
	public string WOJ { get; set; }
	public string POW { get; set; }
	public string GMI { get; set; }
	public string RODZ { get; set; }
	public string NAZWA { get; set; }
	public string NAZWA_DOD { get; set; }
}

public class TercParser
{
	public static void Parse()
	{
		using var file = new FileStream(@"V:\TERC_Adresowy_2026-02-18.csv", FileMode.Open, FileAccess.Read);
		if (!file.CanRead) return;
		using StreamReader sr = new StreamReader(file);
		var res = ParseCsvToClass(sr);
		TercToRegionT(res);
	}

	private static void TercToRegionT(List<TERCItem> itemList)
	{
		List<RegionWoj> woj = [];   //16
		List<RegionPow> pow = [];   //380
		List<RegionGmi> gmi = [];   //2511 (should be 2479) 32 too many
		foreach (var item in itemList)
		{
			if (string.IsNullOrEmpty(item.POW))
			{
				woj.Add(new RegionWoj
				{
					Id = short.Parse(item.WOJ),
					Name = item.NAZWA,
				});
			}
			else if (string.IsNullOrEmpty(item.GMI))
			{
				var wojId = short.Parse(item.WOJ);
				pow.Add(new RegionPow
				{
					WojId = wojId,
					Id = (short)(wojId * 100 + int.Parse(item.POW)),
					Name = item.NAZWA,
				});
			}
			else
			{
				var wojId = short.Parse(item.WOJ);
				var powId = (short)(wojId * 100 + int.Parse(item.POW));
				var gmina = new RegionGmi
				{
					PowId = powId,
					Id = powId * 100 + int.Parse(item.GMI),
					Name = item.NAZWA,
				};
				//add only if it does not exist already (compare the full Compund Key)
				if (!gmi.Where(g => g.Id == gmina.Id).Any())
				{
					gmi.Add(gmina);
				}
			}
		}
	}

	private static List<TERCItem> ParseCsvToClass(StreamReader sr)
	{
		List<TERCItem> items = [];
		string? line;
		while ((line = sr.ReadLine()) != null)
		{
			if (line.StartsWith("WOJ")) continue;
			if (line.Length < 4) continue;
			var parts = line.Split(';');
			items.Add(new TERCItem
			{
				WOJ = parts[0],
				POW = parts[1],
				GMI = parts[2],
				RODZ = parts[3],
				NAZWA = parts[4],
				NAZWA_DOD = parts[5],
			});
		}
		return items;
	}
}
