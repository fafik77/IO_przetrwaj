using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetAllPostsMinimalQueryHandler : IQueryHandler<GetAllPostsMinimalQuery, IEnumerable<PostMinimalCategoryRegion>>
{
	private readonly IPostRepository _postRepository;

	public GetAllPostsMinimalQueryHandler(IPostRepository postRepository)
	{
		_postRepository = postRepository;
	}


	public async Task<IEnumerable<PostMinimalCategoryRegion>> Handle(GetAllPostsMinimalQuery request, CancellationToken cancellationToken)
	{
		return await _postRepository.GetPostsMinimalCategoryRegion(cancellationToken);
	}
}
