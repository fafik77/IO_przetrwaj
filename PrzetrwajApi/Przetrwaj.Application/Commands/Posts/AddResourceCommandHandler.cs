using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Services;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Commands.Posts;

public class AddResourceCommandHandler : ICommandHandler<AddResourceInternallCommand, Post>
{
	private readonly ICategoryRepository _categoryRepository;
	private readonly IPostService _addPostService;

	public AddResourceCommandHandler(ICategoryRepository categoryRepository, IPostService addPostService)
	{
		_categoryRepository = categoryRepository;
		_addPostService = addPostService;
	}

	public async Task<Post> Handle(AddResourceInternallCommand request, CancellationToken cancellationToken)
	{
		IEnumerable<CategoryResource> categories = await _categoryRepository.GetResourcesAsync(cancellationToken);
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
