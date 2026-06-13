using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Queries.Posts;

public class GetAllMatchingPostsQueryHandler : IQueryHandler<GetAllMatchingPostsQuery, IEnumerable<PostOverviewDto>>
{
	private readonly IPostRepository _postRepository;
	private readonly IRegionRepository _regionRepository;
	private readonly IImpedimentsRepository _impedimentsRepository;

	public GetAllMatchingPostsQueryHandler(IPostRepository postRepository, IRegionRepository regionRepository, IImpedimentsRepository impedimentsRepository)
	{
		_postRepository = postRepository;
		_regionRepository = regionRepository;
		_impedimentsRepository = impedimentsRepository;
	}

	public async Task<IEnumerable<PostOverviewDto>> Handle(GetAllMatchingPostsQuery request, CancellationToken cancellationToken)
	{
		var posts = await _postRepository.GetMatchingPostsAsync(request.MatchingPostsFilter, cancellationToken);
		return posts;
	}
}
