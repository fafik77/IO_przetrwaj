using System.Text.Json.Serialization;

namespace Przetrwaj.CommonLibrary.Models;

public class BanInfo
{
	public bool Banned { get; set; } = false;
	[JsonPropertyName("Date")]
	public DateTimeOffset? BanDate { get; set; }
	[JsonPropertyName("Reason")]
	public required string BanReason { get; set; }
	[JsonPropertyName("By")]
	public UserGeneralDto? BannedBy { get; set; }
}
