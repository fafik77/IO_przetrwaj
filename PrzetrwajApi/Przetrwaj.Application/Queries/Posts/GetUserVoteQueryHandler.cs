using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetUserVoteQueryHandler : IQueryHandler<GetUserVoteQuery, VoteDto>
{
	private readonly IPostRepository _postRepository;

	public GetUserVoteQueryHandler(IPostRepository postRepository)
	{
		_postRepository = postRepository;
	}

	public async Task<VoteDto> Handle(GetUserVoteQuery request, CancellationToken cancellationToken)
	{
		if(!await _postRepository.ExistsPostIdAsync(request.PostId)) 
			throw new PostNotFoundException(request.PostId);
		var res = await _postRepository.GetVoteAsync(request.PostId, request.UserId, cancellationToken);
		var dto = (VoteDto)res;
		return dto;
	}
}
