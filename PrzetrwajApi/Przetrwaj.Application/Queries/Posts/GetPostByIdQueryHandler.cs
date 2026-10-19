using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Queries.Posts;

public class GetPostByIdQueryHandler(
    IPostRepository postRepository,
    ICurrentUserService currentUserService) : IQueryHandler<GetPostByIdQuery, PostCompleteDataDto>
{
    public async Task<PostCompleteDataDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var resDto = await postRepository.GetFullROPostByIdAsync(request.Id, cancellationToken);
        if (resDto is null) throw new PostNotFoundException(request.Id);
        var userId = currentUserService.UserId;
        if (userId != null)
            resDto.MyVote = (VoteDto)await postRepository.GetVoteAsync(request.Id, userId, cancellationToken);
        return resDto;
    }
}
