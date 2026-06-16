using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Commands.Posts;
using Przetrwaj.Application.Commands.Posts.Attachments;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Exceptions.Regions;
using Przetrwaj.Domain.Helpers;
using Przetrwaj.Domain.Models.Dtos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Przetrwaj.Application.Services;

public interface IPostService
{
	/// <summary>
	/// Adds a new post, fills in the data, persists in DB
	/// </summary>
	/// <param name="post">the new post to be added (id of wchich does not exist in DB yet)</param>
	/// <param name="addPostData">the data to apply to post</param>
	/// <param name="categories">used to verify that given/custom category is valid</param>
	/// <param name="ClaimsPrincipal">requesting user claims (only Moderator+ can add Resource Posts)</param>
	/// <param name="cancellationToken">Cancellation Token</param>
	/// <returns></returns>
	public Task<Post> FillPostFromDataAndAddAsync(Post post, AddPostCommand addPostData, IEnumerable<Category> categories, ClaimsPrincipal ClaimsPrincipal, CancellationToken cancellationToken);

	/// <summary>
	/// Adds attachments to a post
	/// </summary>
	/// <param name="request">Attachments to add</param>
	/// <param name="post">Tracked post entity to add them to</param>
	/// <param name="cancellationToken">Cancellation Token</param>
	/// <returns></returns>
	/// <exception cref="BadUpdateCommand">Changes could not be saved</exception>
	public Task<AddAttachmentsResult> AddAttachments(AddAttachments addAttachmentsRequest, Post post, CancellationToken cancellationToken);
}

internal class PostService : IPostService
{
	private static readonly Regex InneCategoryRegex = new Regex(
		@"^(inne|inna)(\s+\w+)?$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase
	);

	private readonly IPostRepository _postRepository;
	private readonly IRegionRepository _regionRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IAttachmentRepository _attachmentRepository;
	private readonly AttachmentSettings _attachmentSettings;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public PostService(IPostRepository postRepository, IUnitOfWork unitOfWork, IRegionRepository regionRepository, IAttachmentRepository attachmentRepository, IOptions<AttachmentSettings> options, IHttpContextAccessor contextAccessor)
	{
		_postRepository = postRepository;
		_unitOfWork = unitOfWork;
		_regionRepository = regionRepository;
		_attachmentRepository = attachmentRepository;
		_attachmentSettings = options.Value;
		_httpContextAccessor = contextAccessor;
	}

	public async Task<Post> FillPostFromDataAndAddAsync(Post post, AddPostCommand addPostData, IEnumerable<Category> categories, ClaimsPrincipal ClaimsPrincipal, CancellationToken ct)
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

		if (addPostData.Attachments != null)
		{
			await AddAttachments(addPostData.Attachments, post, ct);
		}

		return post;
	}

	private async Task<(short Woj, short Pow, int Gmi)> RegionFromLocationAsync(LatLong latLong, RegionPrecision regionPrecision, CancellationToken ct)
	{
		var region = await _regionRepository.RegionFromLocationAsync(latLong, ct);
		if (region is null)
			throw new LocationNotInPolandException(latLong);
		return RegionCompoundHelper.RegionSplit(region.Id);
	}

	public async Task<AddAttachmentsResult> AddAttachments(AddAttachments request, Post post, CancellationToken cancellationToken)
	{
		var results = new AddAttachmentsResult
		{
			Status = "success",
			Error = new ErrorDetails { },
			StatusCodeEnum = System.Net.HttpStatusCode.OK,
			Timestamp = DateTimeOffset.UtcNow,
		};


		int attCount = post.Attachments.Count;
		string HttpPath = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host.Value}";
		if (request.Items is null || request.Items.Count == 0)
		{
			results = (AddAttachmentsResult)new NothingChangedException("No Attachments");
			results.Attachments = post.Attachments.Select(a => AttachmentDto.Map(a, HttpPath)!).ToList();
			return results;
		}
		int addedFiles = 0;
		int maxAttachments = _attachmentSettings.MaxFiles;
		long maxAttachmentSize = _attachmentSettings.MaxFileSizeInMB;
		maxAttachmentSize <<= 20;
		if (attCount >= maxAttachments)
			return (AddAttachmentsResult)new TooManyAttachmentsException($"Too many Attachments, max is {maxAttachments}");

		foreach (var itemAtt in request.Items)
		{
			if (attCount >= maxAttachments) break;
			IFormFile file = itemAtt.File;
			///check if file is an image type
			bool isImageType = false;
			foreach (var type in _attachmentSettings.AllowedTypes)
			{
				if (file.ContentType.StartsWith(type, StringComparison.OrdinalIgnoreCase))
				{
					isImageType = true; //is an image
					break;
				}
			}
			if (isImageType == false)
			{
				results.Results.Add((AddAttachmentResult)new InvalidImageException($"{file.FileName}"));
				continue; //not an image
			}
			if (file.Length > maxAttachmentSize)
			{
				results.Results.Add((AddAttachmentResult)new InvalidFileException($"\"{file.FileName}\" is too big. Max size is {_attachmentSettings.MaxFileSizeInMB} MiB"));
				continue; // input image too big
			}
			try
			{
				var proccessedAtt = await ProcessImageAsync(file, cancellationToken);
				using var WebpData = proccessedAtt.WebpData; //we have to dispose of this 
				if (WebpData is null) continue;
				if (WebpData.Length > maxAttachmentSize)
				{
					results.Results.Add((AddAttachmentResult)new InvalidFileException($"Re-encoded \"{file.FileName}\" is too big. Max size is {_attachmentSettings.MaxFileSizeInMB} MiB"));
					continue; // output image too big (for some reason its bigger than the input one)
				}
				var alreadyExists = post.Attachments.FirstOrDefault(a => a.IdAttachment == proccessedAtt.Hash);
				if (alreadyExists != null)
				{
					var item = (AddAttachmentResult)new InvalidFileException($"Re-encoded \"{file.FileName}\" is too big. Max size is {_attachmentSettings.MaxFileSizeInMB} MiB");
					item.AttachmentDto = AttachmentDto.Map(alreadyExists, HttpPath);
					results.Results.Add(item);
					continue; //already exists
				}
				//store attached images as WEBP format (they are web based)
				string fileName = $"{proccessedAtt.Hash}.webp";
				if (false == await _attachmentRepository.SaveAttachmentAsync(WebpData, fileName, cancellationToken))
				{
					var item = (AddAttachmentResult)new InvalidFileException($"Could not save \"{file.FileName}\" as \"{fileName}\"");
					results.Results.Add(item);
					continue; //did not save to file
				}
				var attInDB = new Attachment
				{
					IdPost = post.IdPost,
					IdAttachment = proccessedAtt.Hash,
					AlternateDescription = itemAtt.AltDescription,
					OrderInList = attCount,
				};
				post.Attachments.Add(attInDB);
				++attCount;
				++addedFiles;
				var attAdded = new AddAttachmentResult
				{
					Status = "success",
					StatusCodeEnum = System.Net.HttpStatusCode.Created,
					Timestamp = DateTimeOffset.UtcNow,
					AttachmentDto = AttachmentDto.Map(attInDB, HttpPath)
				};
				results.Results.Add(attAdded);
			}
			catch (Exception ex)
			{
				string errorString = $"Could not process image?: {file.FileName}\n{ex.Message}";
				var item = (AddAttachmentResult)new InvalidFileException(errorString);
				results.Results.Add(item);
				Console.WriteLine(errorString);
			}
		}

		if (addedFiles == 0)
		{
			var fileResults = results.Results;
			results = (AddAttachmentsResult)new NothingChangedException("No valid image files were added");
			results.Results = fileResults;
		}

		try
		{
			//saves the tracked Post item
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
		results.Attachments = post.Attachments.Select(a => AttachmentDto.Map(a, HttpPath)!).ToList();
		results.Timestamp = DateTimeOffset.UtcNow;
		return results;
	}


	/// <summary>
	/// Processes the Image into Webp format
	/// </summary>
	/// <param name="file">Image file</param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="InvalidImageException"/>
	/// <returns>(Hash of output image, Stream of output image has to be closed by the caller)</returns>
	private async Task<(string Hash, Stream WebpData)> ProcessImageAsync(IFormFile file, CancellationToken cancellationToken)
	{
		bool isImageType = false;
		foreach (var type in _attachmentSettings.AllowedTypes)
		{
			if (file.ContentType.StartsWith(type, StringComparison.OrdinalIgnoreCase))
			{
				isImageType = true; //is an image
				break;
			}
		}
		if (isImageType == false)
			throw new InvalidImageException($"{file.FileName}"); // is not an image
		using var fs = file.OpenReadStream();
		// Convert to WebP using ImageSharp
		using var image = await Image.LoadAsync(fs, cancellationToken);
		var outputStream = new MemoryStream();
		// Set WebP encoding options (Quality 0-100)
		var encoder = new WebpEncoder { Quality = 99 };
		await image.SaveAsync(outputStream, encoder, cancellationToken);

		using var sha256 = SHA256.Create();
		outputStream.Seek(0, SeekOrigin.Begin);
		byte[] hashBytes = await sha256.ComputeHashAsync(outputStream, cancellationToken);
		string hashString = Convert.ToHexString(hashBytes);
		outputStream.Seek(0, SeekOrigin.Begin);

		return (hashString, outputStream);
	}
}
