using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Application.Services;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Posts;

public class AddResourceCommandHandler : ICommandHandler<AddResourceInternallCommand, (Post, AddAttachmentsResult?)>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPostService _addPostService;
    private readonly ICurrentUserService _currentUserService;

    public AddResourceCommandHandler(ICategoryRepository categoryRepository, IPostService addPostService, ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _addPostService = addPostService;
        _currentUserService = currentUserService;
    }

    public async Task<(Post, AddAttachmentsResult?)> Handle(AddResourceInternallCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");
        IEnumerable<CategoryResource> categories = await _categoryRepository.GetResourcesAsync(cancellationToken);
        var post = new Post
        {
            Description = request.AddPostCommand.Description ?? string.Empty,
            IdAutor = userId,
            Title = request.AddPostCommand.Title,
            CategoryType = request.Category
        };
        return await _addPostService.FillPostFromDataAndAddAsync(post, request.AddPostCommand, categories, cancellationToken);
    }
}
