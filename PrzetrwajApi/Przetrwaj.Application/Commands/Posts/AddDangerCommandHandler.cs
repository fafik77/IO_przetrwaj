using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Services;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Commands.Posts;

public class AddDangerCommandHandler : ICommandHandler<AddDangerInternallCommand, PostCompleteDataDto>
{
	private readonly ICategoryRepository _categoryRepository;
	private readonly IAddPostService _addPostService;

	public AddDangerCommandHandler(ICategoryRepository categoryRepository, IAddPostService addPostService)
	{
		_categoryRepository = categoryRepository;
		_addPostService = addPostService;
	}

	public async Task<PostCompleteDataDto> Handle(AddDangerInternallCommand request, CancellationToken cancellationToken)
	{
		var categories = await _categoryRepository.GetDangersAsync(cancellationToken);
		var post = new Post
		{
			Description = request.AddPostCommand.Description ?? string.Empty,
			IdAutor = request.IdAutor,
			Title = request.AddPostCommand.Title,
			CategoryType = request.Category
		};
		return await _addPostService.FillPostFromData(post, request.AddPostCommand, categories, request.Claims, cancellationToken);
	}
}
