using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Settings;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Models.Dtos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System.Security.Cryptography;

namespace Przetrwaj.Application.Commands.Posts.Attachments;

public class AddAttachmentsHandler : ICommandHandler<AddAttachmentsInternal, AddAttachmentsResult>
{
	private readonly IPostRepository _postRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IAttachmentRepository _attachmentRepository;
	private readonly AttachmentSettings _attachmentSettings;

	public AddAttachmentsHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IAttachmentRepository attachmentRepository, IOptions<AttachmentSettings> options)
	{
		_postRepository = postRepository;
		_unitOfWork = unitOfWork;
		_httpContextAccessor = httpContextAccessor;
		_attachmentRepository = attachmentRepository;
		_attachmentSettings = options.Value;
	}

	public async Task<AddAttachmentsResult> Handle(AddAttachmentsInternal request, CancellationToken cancellationToken)
	{
		var results = new AddAttachmentsResult
		{
			Status = "success",
			Error = new ErrorDetails { },
			StatusCode = System.Net.HttpStatusCode.OK
		};
		var post = await _postRepository.GetPostWithAttachmentsByIdAsync(request.IdPost, cancellationToken);
		if (post is null)
			return (AddAttachmentsResult)new PostNotFoundException(request.IdPost);

		//check if requester made the Post
		if (!post.IdAutor.Equals(request.IdUser, StringComparison.CurrentCultureIgnoreCase))
			return (AddAttachmentsResult)new NotTheAuthorException($"User: {request.IdUser} did not make the Post: {post.IdAutor}");

		int attCount = post.Attachments.Count;
		//var resDto = new List<AttachmentDto>();
		string HttpPath = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host.Value}";
		if (request.Files is null || request.Files.Count == 0)
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

		for (int att = 0; att != request.Files.Count; ++att)
		{
			if (attCount >= maxAttachments) break;
			IFormFile file = request.Files.ElementAt(att);
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
				results.Results.Add((AddAttachmentResult)new InvalidFileException($"\"{file.FileName}\" is too big. Max size is {_attachmentSettings.MaxFileSizeInMB}"));
				continue; // input image too big
			}
			try
			{
				var proccessedAtt = await ProcessImageAsync(file, cancellationToken);
				using var WebpData = proccessedAtt.WebpData; //we have to dispose of this 
				if (WebpData is null) continue;
				if (WebpData.Length > maxAttachmentSize)
				{
					results.Results.Add((AddAttachmentResult)new InvalidFileException($"Re-encoded \"{file.FileName}\" is too big. Max size is {_attachmentSettings.MaxFileSizeInMB}"));
					continue; // output image too big (for some reason its bigger than the input one)
				}
				var alreadyExists = post.Attachments.FirstOrDefault(a => a.IdAttachment == proccessedAtt.Hash);
				if (alreadyExists != null)
				{
					var item = (AddAttachmentResult)new InvalidFileException($"Re-encoded \"{file.FileName}\" is too big. Max size is {_attachmentSettings.MaxFileSizeInMB}");
					item.AttachmentDto = AttachmentDto.Map(alreadyExists, HttpPath);
					results.Results.Add(item);
					continue; //already exists
				}
				string fileName = $"{proccessedAtt.Hash}.webp";
				if (false == await _attachmentRepository.SaveAttachmentAsync(WebpData, fileName, cancellationToken))
				{
					var item = (AddAttachmentResult)new InvalidFileException($"Could not save \"{file.FileName}\" as \"{fileName}\"");
					results.Results.Add(item);
					continue; //did not save to file
				}
				var attInDB = new Attachment
				{
					IdPost = request.IdPost,
					IdAttachment = proccessedAtt.Hash,
					AlternateDescription = request.AlternateDescriptions?.Count > att ? request.AlternateDescriptions?.ElementAtOrDefault(att) : null,
					OrderInList = attCount,
				};
				post.Attachments.Add(attInDB);
				++attCount;
				++addedFiles;
				var attAdded = new AddAttachmentResult
				{
					Status = "success",
					StatusCode = System.Net.HttpStatusCode.Created,
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

		await _unitOfWork.SaveChangesAsync(cancellationToken);
		results.Attachments = post.Attachments.Select(a => AttachmentDto.Map(a, HttpPath)!).ToList();
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
		// 3. Convert to WebP using ImageSharp
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
