using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.RegionException;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetResourcePostsInRegionQueryHandler
    : IQueryHandler<GetResourcePostsInRegionQuery, IEnumerable<PostOverviewDto>>
{
    private readonly IPostRepository _postRepository;
    private readonly IRegionRepository _regionRepository;

    public GetResourcePostsInRegionQueryHandler(IPostRepository postRepository, IRegionRepository regionRepository)
    {
        _postRepository = postRepository;
        _regionRepository = regionRepository;
    }

    public async Task<IEnumerable<PostOverviewDto>> Handle(GetResourcePostsInRegionQuery request, CancellationToken cancellationToken)
    {
        // 404 jeśli region nie istnieje
        var region = await _regionRepository.GetByIdAsync(request.IdRegion, cancellationToken);
        if (region is null) throw new RegionNotFoundException(request.IdRegion);

        var posts = (await _postRepository.GetResourceByRegionAsync(request.IdRegion, cancellationToken)).ToList();

        foreach (var p in posts)
        {
            p.VoteRatio = p.VoteSum > 0 ? (float)p.VotePositive / p.VoteSum * 100f : 100f;
        }

        return posts;
    }
}
