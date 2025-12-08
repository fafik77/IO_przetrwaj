using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Dtos;

public class CategoryDto
{
    public int IdCategory { get; set; }
    public string Name { get; set; } = null!;
    public CategoryType Type { get; set; }

    public static explicit operator CategoryDto(Category category)
    {
        return new CategoryDto
        {
            IdCategory = category.IdCategory,
            Name = category.Name,
            Type = category.Type
        };
    }
}