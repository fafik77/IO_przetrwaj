using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetDangerCategoryByIdQuery : IQuery<CategoryDto?>
{
    public int IdCategory { get; set; }
}