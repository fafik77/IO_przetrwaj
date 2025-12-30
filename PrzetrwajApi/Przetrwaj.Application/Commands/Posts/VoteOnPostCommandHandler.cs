using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Posts;

namespace Przetrwaj.Application.Commands.Posts;

public class VoteOnPostCommandHandler : ICommandHandler<VoteOnPostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VoteOnPostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VoteOnPostCommand request, CancellationToken cancellationToken)
    {
        var postId = request.IdPost.ToLower();

        // 404 jeśli post nie istnieje
        var post = await _postRepository.GetRWPostByIdAsync(postId, cancellationToken);
        if (post is null) throw new PostNotFoundException(postId);

        // 409 jeśli user już głosował
        var existing = await _postRepository.GetVoteAsync(postId, request.IdUser, cancellationToken);
        if (existing is not null)
            throw new AlreadyVotedException($"{postId}:{request.IdUser}", existing.IsUpvote);

        await _postRepository.AddVoteAsync(new Vote
        {
            IdPost = postId,
            IdUser = request.IdUser,
            IsUpvote = request.IsUpvote
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
