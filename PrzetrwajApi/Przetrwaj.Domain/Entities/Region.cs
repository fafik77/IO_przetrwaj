using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public class Region
{
	[Key]
	public int IdRegion { get; set; }
	//[MaxLength(100)]
	public required string Name { get; set; }
	public double Lat {  get; set; }
	public double Long { get; set; }


	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

	public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}
