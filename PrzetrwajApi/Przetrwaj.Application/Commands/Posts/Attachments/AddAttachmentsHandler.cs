using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Services;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Posts;

namespace Przetrwaj.Application.Commands.Posts.Attachments;

public class AddAttachmentsHandler : ICommandHandler<AddAttachmentsInternal, AddAttachmentsResult>
{
	private readonly IPostRepository _postRepository;
	private readonly IPostService _postService;

	public AddAttachmentsHandler(IPostRepository postRepository, IPostService postService)
	{
		_postRepository = postRepository;
		_postService = postService;
	}

	public async Task<AddAttachmentsResult> Handle(AddAttachmentsInternal request, CancellationToken cancellationToken)
	{
		var post = await _postRepository.GetPostWithAttachmentsByIdAsync(request.IdPost, cancellationToken);
		if (post is null || post.Active == false)
			return (AddAttachmentsResult)new PostNotFoundException(request.IdPost);

		//check if requester made the Post
		if (!post.IdAutor.Equals(request.IdUser, StringComparison.CurrentCultureIgnoreCase))
			return (AddAttachmentsResult)new NotTheAuthorException($"User: {request.IdUser} did not make the Post: {post.IdAutor}");

		return await _postService.AddAttachments(request, post, cancellationToken);
	}
}
