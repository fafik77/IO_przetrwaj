using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Dtos;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetResourceCategoryByIdQuery : IQuery<CategoryDto?>
{
    public int IdCategory { get; set; }
}
