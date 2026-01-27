using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Models.Dtos.Posts;
using System.Text.RegularExpressions;

namespace Przetrwaj.Application.Commands.Posts;

public class AddResourceCommandHandler : ICommandHandler<AddResourceInternallCommand, PostCompleteDataDto>
{
	private readonly IPostRepository _postRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICategoryRepository _categoryRepository;
	private static readonly Regex InneCategoryRegex = new Regex(
		@"^(inne|inna)(\s+\w+)?$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase
	);

	public AddResourceCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
	{
		_postRepository = postRepository;
		_unitOfWork = unitOfWork;
		_categoryRepository = categoryRepository;
	}

	public async Task<PostCompleteDataDto> Handle(AddResourceInternallCommand request, CancellationToken cancellationToken)
	{
		var categories = await _categoryRepository.GetResourcesAsync(cancellationToken);
		if (categories.FirstOrDefault(c => c.IdCategory == request.IdCategory) is null) //check if requested category exists in Resources
		{
			throw new PostNotValidException($"Category: {request.IdCategory} is not a valid Resources");
		}
		var inneCategory = categories.FirstOrDefault(c => InneCategoryRegex.IsMatch(c.Name));
		// Enforce the CustomCategory
		if (!string.IsNullOrEmpty(request.CustomCategory))
		{
			// Rule: Only allow CustomCategory if the selected IdCategory matches the "Inne/Inna" category
			if (inneCategory != null && request.IdCategory == inneCategory.IdCategory)
			{
				// Valid state: The user selected 'Inne' and provided a custom string.
				request.CustomCategory = request.CustomCategory.Trim();
			}
			else
			{
				// Invalid state: User provided a custom name but selected a regular category,
				// or selected nothing that matches "Inne". Clear the custom field.
				request.CustomCategory = null;
			}
		}
		else if (inneCategory != null && inneCategory.IdCategory == request.IdCategory)
		{
			throw new PostNotValidException($"Category: \"{inneCategory.Name}\" requires 'CustomCategory'");
		}
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
		try
		{
			await _postRepository.AddAsync(post, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new PostNotValidException(ex.InnerException.Message);
		}
		return (PostCompleteDataDto)post!;
	}
}
