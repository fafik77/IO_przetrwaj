using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Posts;

public class UpdateCommentCommandHandler : ICommandHandler<UpdateCommentInternalCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPostRepository _postRepository;

    public UpdateCommentCommandHandler(ICurrentUserService currentUserService, IPostRepository postRepository)
    {
        _currentUserService = currentUserService;
        _postRepository = postRepository;
    }

    public async Task Handle(UpdateCommentInternalCommand request, CancellationToken cancellationToken)
    {
        var comment = await _postRepository.GetUserCommentAsync(request.IdComment, cancellationToken)
            ?? throw new PostNotFoundException(request.IdComment);
        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");
        if (!comment.IdAutor.Equals(userId, StringComparison.OrdinalIgnoreCase))
            throw new NotTheAuthorException($"not the author of {request.IdComment}");

        comment.Comment = request.Comment;
        _postRepository.UpdateComment(comment);
    }
}