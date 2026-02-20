using CsvHelper;
using CsvHelper.Configuration;
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
		using StreamReader sr = new StreamReader(file);
		using var reader = new CsvReader(sr,
			new CsvConfiguration(cultureInfo: System.Globalization.CultureInfo.InvariantCulture)
			{
				Delimiter = ";"
			});
		var res = reader.GetRecords<TERCItem>();
		TercToRegionT(res);
	}
	public struct TercRegionResults
	{
		public List<RegionWoj> woj; //16
		public List<RegionPow> pow; //380
		public List<RegionGmi> gmi; //2511 (should be 2479) 32 too many
		public TercRegionResults()
		{
			woj = [];
			pow = [];
			gmi = [];
		}
	}

	private static TercRegionResults TercToRegionT(IEnumerable<TERCItem> itemList)
	{
		var results = new TercRegionResults();
		foreach (var item in itemList)
		{
			if (string.IsNullOrEmpty(item.POW))
			{
				results.woj.Add(new RegionWoj
				{
					Id = short.Parse(item.WOJ),
					Name = item.NAZWA,
				});
			}
			else if (string.IsNullOrEmpty(item.GMI))
			{
				var wojId = short.Parse(item.WOJ);
				results.pow.Add(new RegionPow
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
				if (!results.gmi.Where(g => g.Id == gmina.Id).Any())
				{
					results.gmi.Add(gmina);
				}
			}
		}
		//so there is 32 gmi too many
		//var freq = results.gmi.GroupBy(x => x.Name).OrderByDescending(x => x.Count()).ToDictionary(x => x.Key, x => x.Count());
		return results;
	}
}
