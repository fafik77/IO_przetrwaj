using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Posts;

namespace Przetrwaj.Application.Commands.Posts;

public class MarkPostAsInactiveCommandHandler : ICommandHandler<MarkPostAsInactiveCommand>
{
	private readonly IPostRepository _postRepository;
	private readonly IUnitOfWork _unitOfWork;

	public MarkPostAsInactiveCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
	{
		_postRepository = postRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task Handle(MarkPostAsInactiveCommand request, CancellationToken cancellationToken)
	{
		var post = await _postRepository.GetRWPostByIdAsync(request.PostId, cancellationToken);
		if (post is null)
			throw new PostNotFoundException(request.PostId);
		post.Active = false;
		await _unitOfWork.SaveChangesAsync(cancellationToken);
	}
}
