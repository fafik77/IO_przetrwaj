using System.Text.Json;
using System.Text.Json.Serialization;

namespace Przetrwaj.CommonLibrary.Models;

public class BanInfo
{
	public bool Banned { get; set; } = false;
	[JsonPropertyName("Date")]
	public DateTimeOffset? BanDate { get; set; }
	[JsonPropertyName("Reason")]
	public required string BanReason { get; set; }
	public required string BannedById { get; set; }
	[JsonPropertyName("By")]
	public UserGeneralDto? BannedBy { get; set; }
	/// <summary>
	/// Stringifies this into JSON
	/// </summary>
	/// <returns>JSON string</returns>
	public override string ToString()
	{
		var options = new JsonSerializerOptions
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			WriteIndented = false
		};
		return JsonSerializer.Serialize(this, options);
	}
}
