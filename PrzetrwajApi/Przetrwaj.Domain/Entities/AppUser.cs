using Microsoft.AspNetCore.Identity;

namespace Przetrwaj.Domain.Entities;

public class AppUser : IdentityUser
{
	public AppUser() : base() { }

	//[MaxLength(24)]
	public string? Name { get; set; }
	//[MaxLength(24)]
	public string? Surname { get; set; }
	//public int IdRegion { get; set; }
	public DateTimeOffset? BanDate { get; set; }
	//[MaxLength(300)]
	public string? BanReason { get; set; }
	//[MaxLength(36)]
	public string? BannedById { get; set; }
	public DateTimeOffset RegistrationDate { get; set; } = DateTimeOffset.UtcNow;
	public bool ModeratorRolePending { get; set; }
	public DateTimeOffset? ModeratorSince { get; set; }

	/// <summary>
	/// Publicly visible info
	/// </summary>
	public short PowiatId { get; set; }
	/// <summary>
	/// private preference for Post display sorting rules
	/// </summary>
	public int? GminaId { get; set; }
	public int Impediments { get; set; } = 0;

	public virtual RegionPow RegionPowNavigation { get; set; } = null!;
	public virtual RegionGmi? RegionGmiNavigation { get; set; }
	public virtual ICollection<UserComment> Comments { get; set; } = new List<UserComment>();
	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
