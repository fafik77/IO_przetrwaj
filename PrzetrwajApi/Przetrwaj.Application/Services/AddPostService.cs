using Przetrwaj.Application.Commands.Posts;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Domain.Models.Dtos.Posts;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Przetrwaj.Application.Services;

public interface IAddPostService
{
	public Task<PostCompleteDataDto> FillPostFromData(Post post, AddPostCommand addPostData, IEnumerable<Category> categories, IEnumerable<Claim> Claims, CancellationToken cancellationToken);
	public (short Woj, short Pow, int Gmi) RegionFromLocation(LatLong latLong, RegionPrecision regionPrecision);
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

	public async Task<PostCompleteDataDto> FillPostFromData(Post post, AddPostCommand addPostData, IEnumerable<Category> categories, IEnumerable<Claim> Claims, CancellationToken cancellationToken)
	{
		#region Claims to RegionPrecision Visibility check
		switch (addPostData.RegionPrecision)
		{
			case RegionPrecision.PL:
			case RegionPrecision.WOJ:
				//case RegionPrecision.POW:
				{
					throw new NotImplementedException();
					//if(Claims.Contains(UserRoles.Moderator))
					break;
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

		var region = RegionFromLocation(addPostData.LatLong, addPostData.RegionPrecision);
		switch (addPostData.RegionPrecision)
		{
			case RegionPrecision.PL:
				post.IdGmiOnly = 0;
				break;
			case RegionPrecision.WOJ:
				post.IdWojOnly = region.Woj;
				break;
			case RegionPrecision.POW:
				post.IdPowOnly = region.Pow;
				goto default;
			case RegionPrecision.GMI:
				post.IdGmiOnly = region.Gmi;
				goto default;
			default:
				post.Lat = addPostData.LatLong.Lat;
				post.Long = addPostData.LatLong.Long;
				break;
		}

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

	public (short Woj, short Pow, int Gmi) RegionFromLocation(LatLong latLong, RegionPrecision regionPrecision)
	{
		(short Woj, short Pow, int Gmi) Region = new();
		throw new NotImplementedException();
		throw new LocationOutsideOfPolandBoundsException($"Lat: {latLong.Lat}, Long: {latLong.Long}");
		return Region;
	}

}
