using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Services;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Posts;

public class AddDangerCommandHandler : ICommandHandler<AddDangerInternallCommand, Post>
{
	private readonly ICategoryRepository _categoryRepository;
	private readonly IPostService _addPostService;

	public AddDangerCommandHandler(ICategoryRepository categoryRepository, IPostService addPostService)
	{
		_categoryRepository = categoryRepository;
		_addPostService = addPostService;
	}

	public async Task<Post> Handle(AddDangerInternallCommand request, CancellationToken cancellationToken)
	{
		var categories = await _categoryRepository.GetDangersAsync(cancellationToken);
		var post = new Post
		{
			Description = request.AddPostCommand.Description ?? string.Empty,
			IdAutor = request.IdAutor,
			Title = request.AddPostCommand.Title,
			CategoryType = request.Category
		};
		return await _addPostService.FillPostFromDataAndAddAsync(post, request.AddPostCommand, categories, request.ClaimsPrincipal, cancellationToken);
	}
}
