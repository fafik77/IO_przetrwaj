using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetUserVoteQueryHandler : IQueryHandler<GetUserVoteQuery, VoteDto>
{
    private readonly IPostRepository _postRepository;
    private readonly ICurrentUserService _currentUserService;


    public GetUserVoteQueryHandler(IPostRepository postRepository, ICurrentUserService currentUserService)
    {
        _postRepository = postRepository;
        _currentUserService = currentUserService;
    }

    public async Task<VoteDto> Handle(GetUserVoteQuery request, CancellationToken cancellationToken)
    {
        if (!await _postRepository.ExistsPostIdAsync(request.PostId))
            throw new PostNotFoundException(request.PostId);
        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");
        var res = await _postRepository.GetVoteAsync(request.PostId, userId, cancellationToken);
        var dto = (VoteDto)res;
        return dto;
    }
}
