using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Posts;

public class DeletePostCommandHandler(ICurrentUserService currentUserService, IPostRepository postRepository) : ICommandHandler<DeletePostCommand>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IPostRepository _postRepository = postRepository;

    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");
        var post = await _postRepository.GetRWPostByIdAsync(request.Id, cancellationToken)
            ?? throw new PostNotFoundException(request.Id);
        if (!post.IdAutor.Equals(userId, StringComparison.OrdinalIgnoreCase))
            throw new NotTheAuthorException($"not the author of {request.Id}");

        post.Active = false;
        _postRepository.Update(post, cancellationToken);
    }
}