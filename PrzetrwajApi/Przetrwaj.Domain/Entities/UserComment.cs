using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public partial class UserComment
{
	[Key]
	[MaxLength(64)]
	public required string IdComment { get; set; }

	[MaxLength(64)]
	public required string IdPost { get; set; }

	[MaxLength(64)]
	public required string IdAutor { get; set; }

	//[MaxLength(1000)]
	public string Comment { get; set; } = null!;

	public DateTime DateCreated { get; set; }


	public virtual AppUser IdAutorNavigation { get; set; } = null!;
	public virtual Post IdPostNavigation { get; set; } = null!;
}
