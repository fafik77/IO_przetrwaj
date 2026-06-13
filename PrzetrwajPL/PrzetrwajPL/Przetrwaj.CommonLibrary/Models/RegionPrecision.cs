using System.Text.Json.Serialization;

namespace Przetrwaj.CommonLibrary.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RegionPrecision
{
	PL = 0,
	WOJ = 1,
	POW = 2,
	GMI = 3,
}
