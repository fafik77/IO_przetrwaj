using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetAllCustomCategoriesQuery : IQuery<IEnumerable<PostOverviewDto>> { }
