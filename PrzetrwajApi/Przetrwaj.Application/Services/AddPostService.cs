using Przetrwaj.Application.Commands.Posts;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Exceptions.Regions;
using Przetrwaj.Domain.Helpers;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Domain.Models.Dtos.Posts;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Przetrwaj.Application.Services;

public interface IAddPostService
{
	public Task<PostCompleteDataDto> FillPostFromDataAndAddAsync(Post post, AddPostCommand addPostData, IEnumerable<Category> categories, ClaimsPrincipal ClaimsPrincipal, CancellationToken cancellationToken);
	public Task<(short Woj, short Pow, int Gmi)> RegionFromLocationAsync(LatLong latLong, RegionPrecision regionPrecision, CancellationToken ct);
}

internal class AddPostService : IAddPostService
{
	private readonly IPostRepository _postRepository;
	private readonly IRegionRepository _regionRepository;
	private readonly IUnitOfWork _unitOfWork;
	private static readonly Regex InneCategoryRegex = new Regex(
		@"^(inne|inna)(\s+\w+)?$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase
	);

	public AddPostService(IPostRepository postRepository, IUnitOfWork unitOfWork, IRegionRepository regionRepository)
	{
		_postRepository = postRepository;
		_unitOfWork = unitOfWork;
		_regionRepository = regionRepository;
	}

	public async Task<PostCompleteDataDto> FillPostFromDataAndAddAsync(Post post, AddPostCommand addPostData, IEnumerable<Category> categories, ClaimsPrincipal ClaimsPrincipal, CancellationToken ct)
	{
		#region Claims to RegionPrecision Visibility check
		switch (addPostData.RegionPrecision)
		{
			case RegionPrecision.PL:
			case RegionPrecision.WOJ:
			case RegionPrecision.POW:
				{
					if (ClaimsPrincipal.IsInRole(UserRoles.Moderator) ||
						ClaimsPrincipal.IsInRole(UserRoles.Admin))
						break;
					throw new PermissionDeniedException("Not enough privilages");
				}
			default: break;
		}
		#endregion Claims

		#region Custom Category
		if (categories.FirstOrDefault(c => c.IdCategory == addPostData.IdCategory) is null) //check if requested category exists in Resources
		{
			throw new PostNotValidException($"Category: {addPostData.IdCategory} is not a valid Resources");
		}
		var inneCategory = categories.FirstOrDefault(c => InneCategoryRegex.IsMatch(c.Name));
		// Enforce the CustomCategory
		if (!string.IsNullOrEmpty(addPostData.CustomCategory))
		{
			// Rule: Only allow CustomCategory if the selected IdCategory matches the "Inne/Inna" category
			if (inneCategory != null && addPostData.IdCategory == inneCategory.IdCategory)
			{
				// Valid state: The user selected 'Inne' and provided a custom string.
				addPostData.CustomCategory = addPostData.CustomCategory.Trim();
			}
			else
			{
				// Invalid state: User provided a custom name but selected a regular category,
				// or selected nothing that matches "Inne". Clear the custom field.
				addPostData.CustomCategory = null;
			}
		}
		else if (inneCategory != null && inneCategory.IdCategory == addPostData.IdCategory)
		{
			throw new PostNotValidException($"Category: \"{inneCategory.Name}\" requires 'CustomCategory'");
		}
		#endregion //CustomCategory
		post.IdCategory = addPostData.IdCategory;
		post.CustomCategory = addPostData.CustomCategory ?? string.Empty;

		var (Woj, Pow, Gmi) = await RegionFromLocationAsync(addPostData.LatLong, addPostData.RegionPrecision, ct);
		switch (addPostData.RegionPrecision)
		{
			case RegionPrecision.PL:
				post.IdRegion = 0;
				break;
			case RegionPrecision.WOJ:
				post.IdRegion = Woj;
				break;
			case RegionPrecision.POW:
				post.IdRegion = Pow;
				goto default;
			case RegionPrecision.GMI:
				post.IdRegion = Gmi;
				goto default;
			default:
				post.Lat = addPostData.LatLong.Lat;
				post.Long = addPostData.LatLong.Long;
				break;
		}

		try
		{
			await _postRepository.AddAsync(post, ct);
			await _unitOfWork.SaveChangesAsync(ct);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new PostNotValidException(ex.InnerException.Message);
		}
		return (PostCompleteDataDto)post!;
	}

	public async Task<(short Woj, short Pow, int Gmi)> RegionFromLocationAsync(LatLong latLong, RegionPrecision regionPrecision, CancellationToken ct)
	{
		var region = await _regionRepository.RegionFromLocationAsync(latLong, ct);
		if (region is null)
			throw new LocationNotInPolandException(latLong);
		return RegionCompoundHelper.RegionSplit(region.Id);
	}

}
