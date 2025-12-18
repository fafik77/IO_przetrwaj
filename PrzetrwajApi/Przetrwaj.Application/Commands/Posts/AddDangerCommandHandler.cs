using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos.Posts;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Posts;

public class AddDangerCommandHandler : ICommandHandler<AddDangerInternallCommand, PostCompleteDataDto>
{
	private readonly IPostRepository _postRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AddDangerCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
	{
		_postRepository = postRepository;
		_unitOfWork = unitOfWork;
	}


	public async Task<PostCompleteDataDto> Handle(AddDangerInternallCommand request, CancellationToken cancellationToken)
	{
		var post = new Post
		{
			Description = request.Description ?? string.Empty,
			IdAutor = request.IdAutor,
			Title = request.Title,
			Category = request.Category,
			IdRegion = request.IdRegion,
			CustomCategory = request.CustomCategory ?? string.Empty,
			IdCategory = request.IdCategory,
		};
		await _postRepository.AddAsync(post, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return (PostCompleteDataDto)post;
	}
}
