using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public class Vote
{
	#region UniquePair(IdPost,IdUser)
	[MaxLength(36)]
	public required string IdPost { get; set; }
	[MaxLength(36)]
	public required string IdUser { get; set; }
	#endregion


	public virtual Post IdPostNavigation { get; set; } = null!;

	public virtual AppUser IdUserNavigation { get; set; } = null!;
}
