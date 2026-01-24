using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Domain.Models;

public class BanInfo
{
	public bool Banned { get; set; } = false;
	public DateTimeOffset? BanDate { get; set; }
	public required string BanReason { get; set; }
	public required string BannedById { get; set; }
	public UserGeneralDto? BannedBy { get; set; }
}
