using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Posts;

public class AddCommentCommandHandler : ICommandHandler<AddCommentInternalCommand, CommentDto>
{
	private readonly IPostRepository _postRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AddCommentCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
	{
		_postRepository = postRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<CommentDto> Handle(AddCommentInternalCommand request, CancellationToken cancellationToken)
	{
		var comment = new UserComment
		{
			IdAutor = request.IdAutor,
			IdPost = request.IdPost,
			Comment = request.Comment,
		};
		var res = await _postRepository.AddCommentAsync(comment, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return (CommentDto)res;
	}
}
