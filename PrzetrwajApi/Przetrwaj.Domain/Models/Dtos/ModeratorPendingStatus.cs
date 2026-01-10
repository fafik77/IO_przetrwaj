namespace Przetrwaj.Domain.Models.Dtos;

public class ModeratorPendingStatus
{
	public required string Id { get; set; }
	public required string Email { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
	public int RegionId { get; set; }
	public required string RegionName { get; set; }
}
