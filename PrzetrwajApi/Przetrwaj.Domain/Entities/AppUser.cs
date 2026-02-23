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
	//public bool Zsight { get; set; }
	//public bool Zhearing { get; set; }
	//public bool Zmovement { get; set; }
	//public bool Zspeach { get; set; }
	//public bool Zbody { get; set; }
	///Remove the `Banned` field, its unnecessary with `BanDate?`
	//public bool Banned { get; set; } = false;

	//[ForeignKey(nameof(IdRegion))]
	//public virtual Region IdRegionNavigation { get; set; } = null!;
	public virtual RegionPow RegionPowNavigation { get; set; } = null!;
	public virtual RegionGmi? RegionGmiNavigation { get; set; }
	public virtual ICollection<UserComment> Comments { get; set; } = new List<UserComment>();
	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
