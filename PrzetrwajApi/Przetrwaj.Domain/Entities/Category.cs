using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public abstract class Category
{
	[Key]
	public int IdCategory { get; set; }

	//[MaxLength(100)]
	public required string Name { get; set; }

	public CategoryType Type { get; set; }
	//[MaxLength(70)]
	public string? CategoryIcon { get; set; }

	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
