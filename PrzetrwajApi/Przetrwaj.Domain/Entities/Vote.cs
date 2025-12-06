using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public partial class Vote
{
	[Key]
	[MaxLength(64)]
	public string IdVote { get; set; }
	#region UniquePair(IdPost,IdUser)
	[MaxLength(64)]
	public required string IdPost { get; set; }
	[MaxLength(64)]
	public required string IdUser { get; set; }
	#endregion


	public virtual Post IdPostNavigation { get; set; } = null!;

	public virtual AppUser IdUserNavigation { get; set; } = null!;
}
