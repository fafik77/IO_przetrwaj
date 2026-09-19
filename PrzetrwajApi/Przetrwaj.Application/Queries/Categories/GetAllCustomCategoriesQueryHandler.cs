using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Quaries.Categories;

public class GetAllCustomCategoriesQueryHandler(ICategoryRepository repo, IPostRepository postRepository)
    : IQueryHandler<GetAllCustomCategoriesQuery, IEnumerable<PostOverviewDto>>
{
    public async Task<IEnumerable<PostOverviewDto>> Handle(GetAllCustomCategoriesQuery request, CancellationToken cancellationToken)
    {
        var customCategoryPosts = await postRepository.GetPostsWithCustomCategoryAsync(cancellationToken);
        return customCategoryPosts;
    }
}
