using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetAllPostsMinimalQuery : IQuery<IEnumerable<PostMinimalCategoryRegion>>
{ }
