using Przetrwaj.Domain.Models.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Przetrwaj.Domain.Models;

public class BanInfo
{
	public bool Banned { get; set; } = false;
	[JsonPropertyName("Date")]
	public DateTimeOffset? BanDate { get; set; }
	[JsonPropertyName("Reason")]
	public required string BanReason { get; set; }
	//[JsonIgnore(Condition = JsonIgnoreCondition.Always)]
	[JsonPropertyName("ById")]
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
			// 1. Prevents crashing if objects refer to each other (Circular Reference)
			ReferenceHandler = ReferenceHandler.IgnoreCycles,
			// 2. Ensures nulls are written as "null" instead of skipped
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			// 3. Compact mode (No spaces/newlines)
			WriteIndented = false
		};
		// JsonSerializer defaults to compact (no whitespace) 
		// unless WriteIndented = true.
		return JsonSerializer.Serialize(this, options);
	}
}
