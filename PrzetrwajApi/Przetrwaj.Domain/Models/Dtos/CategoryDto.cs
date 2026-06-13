using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos;

public record CategoryDto
{
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public CategoryType Type { get; set; }

	public static explicit operator CategoryDto?(Category? category)
	{
		return category is null ? null : new CategoryDto
		{
			Id = category.IdCategory,
			Name = category.Name,
			Type = category.Type
		};
	}
}