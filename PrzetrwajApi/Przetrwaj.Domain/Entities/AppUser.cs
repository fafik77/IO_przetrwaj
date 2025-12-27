using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Przetrwaj.Domain.Entities;

public class AppUser : IdentityUser
{
	public AppUser() : base() { }

	[MaxLength(24)]
	public string? Name { get; set; }
	[MaxLength(24)]
	public string? Surname { get; set; }
	public int IdRegion { get; set; }
	public bool Banned { get; set; } = false;
	public DateTimeOffset? BanDate { get; set; }
	//[MaxLength(300)]
	public string? BanReason { get; set; }
	[MaxLength(36)]
	public string? BannedById { get; set; }
	public DateTimeOffset RegistrationDate { get; set; } = DateTimeOffset.UtcNow;
	public bool ModeratorRolePending { get; set; }
	public DateTimeOffset? ModeratorSince { get; set; }


	[ForeignKey(nameof(IdRegion))]
	public virtual Region IdRegionNavigation { get; set; } = null!;
	public virtual ICollection<UserComment> Comments { get; set; } = new List<UserComment>();
	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
