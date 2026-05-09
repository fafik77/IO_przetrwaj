using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetDangerCategoriesQuery : IQuery<IEnumerable<CategoryDto>> { }
