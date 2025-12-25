using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Posts;

public class AddCommentCommandHandler : ICommandHandler<AddCommentInternalCommand, CommentDto>
{
	private readonly IUserRepository _userRepository;
	private readonly IPostRepository _postRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AddCommentCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
	{
		_postRepository = postRepository;
		_unitOfWork = unitOfWork;
		_userRepository = userRepository;
	}

	public async Task<CommentDto> Handle(AddCommentInternalCommand request, CancellationToken cancellationToken)
	{
		var postExists = await _postRepository.ExistsPostIdAsync(request.IdPost, cancellationToken);
		if (false == postExists) throw new PostNotFoundException(request.IdPost);
		var user = await _userRepository.GetByIdAsync(request.IdAutor, cancellationToken);
		var comment = new UserComment
		{
			IdAutor = request.IdAutor,
			IdPost = request.IdPost,
			Comment = request.Comment,
		};
		var res = await _postRepository.AddCommentAsync(comment, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		var dto = (CommentDto)res;
		dto.Autor = (UserGeneralDto?)user!;
		return dto;
	}
}
