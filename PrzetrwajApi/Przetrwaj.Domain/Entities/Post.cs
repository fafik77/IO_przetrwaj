using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public class Post
{
	[Key]
	[MaxLength(64)]
	public virtual string IdPost { get; set; } = default!;

	//[MaxLength(200)]
	public required string Title { get; set; }

	//[MaxLength(2000)]
	public required string Description { get; set; }

	public CategoryType Category { get; set; }

	public int IdCategory { get; set; }
	//[MaxLength(100)]
	public string CustomCategory { get; set; } = string.Empty;

	public int IdRegion { get; set; }

	[MaxLength(64)]
	public required string IdAutor { get; set; }

	public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;

	public bool Active { get; set; } = true;


	public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

	public virtual ICollection<UserComment> Comments { get; set; } = new List<UserComment>();

	public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();


	public virtual AppUser IdAutorNavigation { get; set; } = null!;

	public virtual Category IdCategoryNavigation { get; set; } = null!;

	public virtual Region IdRegionNavigation { get; set; } = null!;

	/// <summary>
	/// Ctor that automatically fills in: IdPost, DateCreated, Active
	/// </summary>
	public Post() : base()
	{
		IdPost = Guid.NewGuid().ToString();
		DateCreated = DateTimeOffset.UtcNow;
		Active = true;
	}
}
