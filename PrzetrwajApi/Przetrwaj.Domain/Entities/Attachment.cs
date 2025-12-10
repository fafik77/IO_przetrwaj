using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public partial class Attachment
{
	[Key]
	[MaxLength(64)]
	public required string IdAttachment { get; set; }
	[MaxLength(64)]
	public required string IdPost { get; set; }
	[MaxLength(200)]
	public string? AlternateDescription { get; set; }
	/// <summary>
	/// PN: i think this will be necessary
	/// </summary>
	public int OrderInList { get; set; }


	public virtual Post IdPostNavigation { get; set; } = null!;
}
