using System.Text.Json.Serialization;

namespace Przetrwaj.CommonLibrary.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum CategoryType
	{
		Danger,
		Resource,
	}
}
