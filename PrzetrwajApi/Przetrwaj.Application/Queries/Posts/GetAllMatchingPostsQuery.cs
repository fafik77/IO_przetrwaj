using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Queries.Posts;

public class GetAllMatchingPostsQuery : IQuery<IEnumerable<PostOverviewDto>>
{
	public required MatchingPostsFilter MatchingPostsFilter;
}
