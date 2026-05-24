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
	/// private preference for Post display sorting rules.
	/// publicly visible is only Woj
	/// </summary>
	public int? GminaId { get; set; }
	public int Impediments { get; set; } = 0;

	public virtual Region? RegionNavigation { get; set; }
	public virtual ICollection<UserComment> Comments { get; set; } = new List<UserComment>();
	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
