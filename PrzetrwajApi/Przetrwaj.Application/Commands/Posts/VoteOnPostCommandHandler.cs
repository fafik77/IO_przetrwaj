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
		// 404 jeśli post nie istnieje
		if (!await _postRepository.ExistsPostIdAsync(request.IdPost, cancellationToken))
			throw new PostNotFoundException(request.IdPost);

		// 409 jeśli user już głosował
		var existing = await _postRepository.GetVoteAsync(request.IdPost, request.IdUser, cancellationToken);
		if (existing != null)
			throw new AlreadyVotedException($"{request.IdPost}:{request.IdUser}", existing.IsUpvote);

		await _postRepository.AddVoteAsync(new Vote
		{
			IdPost = request.IdPost.ToLower(),
			IdUser = request.IdUser.ToLower(),
			IsUpvote = request.IsUpvote
		}, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);
	}
}
