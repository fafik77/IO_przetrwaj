using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Domain.Entities;

public abstract class Category
{
	[Key]
	public int IdCategory { get; set; }

	//[MaxLength(100)]
	public required string Name { get; set; }

	public CategoryType Type { get; set; }
	static public readonly string Type_ = "Type";
	//[MaxLength(70)]
	public string? CategoryIcon { get; set; }
	public int Impediments { get; set; } = 0;

	public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}

public enum CategoryType
{
	Danger,
	Resource,
}

public enum CategoryTypeFilter
{
	Danger,
	Resource,
	Both
}
