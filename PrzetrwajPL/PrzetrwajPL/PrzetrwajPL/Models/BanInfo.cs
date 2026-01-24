using PrzetrwajPL.Models;
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
	[JsonIgnore(Condition = JsonIgnoreCondition.Always)]
	public required string BannedById { get; set; }
	[JsonPropertyName("By")]
	public UserGeneralDto? BannedBy { get; set; }
	/// <summary>
	/// Stringifies this into JSON
	/// </summary>
	/// <returns>JSON string</returns>
	public override string ToString()
	{
		// JsonSerializer defaults to compact (no whitespace) 
		// unless WriteIndented = true.
		return JsonSerializer.Serialize(this);
	}
}
