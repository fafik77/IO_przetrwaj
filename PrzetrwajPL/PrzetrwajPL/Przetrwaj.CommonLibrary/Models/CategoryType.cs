using System.Text.Json.Serialization;

namespace PrzetrwajPL.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CategoryType
{
	Danger,
	Resource,
}
