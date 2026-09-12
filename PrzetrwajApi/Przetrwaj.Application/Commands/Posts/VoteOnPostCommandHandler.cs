using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Posts;

public class VoteOnPostCommandHandler : ICommandHandler<VoteOnPostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public VoteOnPostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(VoteOnPostCommand request, CancellationToken cancellationToken)
    {
        // 404 jeśli post nie istnieje
        if (!await _postRepository.ExistsActivePostIdAsync(request.IdPost, cancellationToken))
            throw new PostNotFoundException(request.IdPost);

        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");

        // 409 jeśli user już głosował
        var existing = await _postRepository.GetVoteAsync(request.IdPost, userId, cancellationToken);
        if (existing != null)
            throw new AlreadyVotedException($"{request.IdPost}:{userId}", existing.IsUpvote);

        await _postRepository.AddVoteAsync(new Vote
        {
            IdPost = request.IdPost.ToLower(),
            IdUser = userId.ToLower(),
            IsUpvote = request.IsUpvote
        }, cancellationToken);
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            throw new BadUpdateCommand(ex.InnerException.Message);
        }
    }
}
