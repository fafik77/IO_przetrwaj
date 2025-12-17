using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public class CategoryDto
{
	public int IdCategory { get; set; }
	public string Name { get; set; } = null!;
	public CategoryType Type { get; set; }

	public static explicit operator CategoryDto?(Category? category)
	{
		return category is null ? null : new CategoryDto
		{
			IdCategory = category.IdCategory,
			Name = category.Name,
			Type = category.Type
		};
	}
}