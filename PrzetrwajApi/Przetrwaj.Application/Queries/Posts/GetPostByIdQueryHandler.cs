using Microsoft.AspNetCore.Http;
using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Helpers;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Queries.Posts;

public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostCompleteDataDto>
{
    private readonly IPostRepository _postRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;

    public GetPostByIdQueryHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService)
    {
        _postRepository = postRepository;
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
    }


    public async Task<PostCompleteDataDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var resDto = await _postRepository.GetFullROPostByIdAsync(request.Id, cancellationToken);
        if (resDto is null) throw new PostNotFoundException(request.Id);
        var resourcePath = HttpPathHelper.HttpPath(_httpContextAccessor);
        foreach (var attachment in resDto.Attachments)
        {
            if (attachment != null)
                attachment.BaseUrl = resourcePath;
        }
        var userId = _currentUserService.UserId;
        if (userId != null)
            resDto.MyVote = (VoteDto)await _postRepository.GetVoteAsync(request.Id, userId, cancellationToken);
        return resDto;
    }
}
