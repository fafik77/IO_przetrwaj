using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public partial class UserComment
{
	[Key]
	[MaxLength(36)]
	public virtual string IdComment { get; set; } = default!;

	[MaxLength(36)]
	public required string IdPost { get; set; }

	[MaxLength(36)]
	public required string IdAutor { get; set; }

	//[MaxLength(1000)]
	public string Comment { get; set; } = null!;

	public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;


	public virtual AppUser IdAutorNavigation { get; set; } = null!;
	public virtual Post IdPostNavigation { get; set; } = null!;

	/// <summary>
	/// Ctor that automatically fills in: IdComment, DateCreated
	/// </summary>
	public UserComment()
	{
		IdComment = Guid.NewGuid().ToString();
		DateCreated = DateTimeOffset.UtcNow;
	}
}
