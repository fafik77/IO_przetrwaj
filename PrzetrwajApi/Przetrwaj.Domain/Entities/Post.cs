using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public partial class Post
{
	[Key]
	[MaxLength(64)]
	public required string IdPost { get; set; }

	[MaxLength(200)]
	public string Title { get; set; } = null!;

	[MaxLength(2000)]
	public string Description { get; set; } = null!;

	public CategoryType Category { get; set; }

	public int IdCategory { get; set; }

	public int IdRegion { get; set; }

	[MaxLength(64)]
	public required string IdAutor { get; set; }

	public DateTime DateCreated { get; set; }

	public bool Active { get; set; }


	public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

	public virtual ICollection<UserComment> Comments { get; set; } = new List<UserComment>();

	public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();


	public virtual AppUser IdAutorNavigation { get; set; } = null!;

	public virtual Category IdCategoryNavigation { get; set; } = null!;

	public virtual Region IdRegionNavigation { get; set; } = null!;
}
