using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Services;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Posts.Attachments;

public class AddAttachmentsHandler : ICommandHandler<AddAttachmentsInternal, AddAttachmentsResult>
{
    private readonly IPostRepository _postRepository;
    private readonly IPostService _postService;
    private readonly ICurrentUserService _currentUserService;

    public AddAttachmentsHandler(IPostRepository postRepository, IPostService postService, ICurrentUserService currentUserService)
    {
        _postRepository = postRepository;
        _postService = postService;
        _currentUserService = currentUserService;
    }

    public async Task<AddAttachmentsResult> Handle(AddAttachmentsInternal request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetPostWithAttachmentsByIdAsync(request.IdPost, cancellationToken);
        if (post is null || post.Active == false)
            return (AddAttachmentsResult)new PostNotFoundException(request.IdPost);

        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");
        //check if requester made the Post
        if (!post.IdAutor.Equals(userId, StringComparison.CurrentCultureIgnoreCase))
            return (AddAttachmentsResult)new NotTheAuthorException($"User: {userId} did not make the Post: {post.IdAutor}");

        return await _postService.AddAttachments(request, post, cancellationToken);
    }
}
