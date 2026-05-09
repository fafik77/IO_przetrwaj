using Microsoft.AspNetCore.Http;
using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostCompleteDataDto>
{
	private readonly IPostRepository _postRepository;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public GetPostByIdQueryHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor)
	{
		_postRepository = postRepository;
		_httpContextAccessor = httpContextAccessor;
	}


	public async Task<PostCompleteDataDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
	{
		var resDto = await _postRepository.GetFullROPostByIdAsync(request.Id, cancellationToken);
		if (resDto is null) throw new PostNotFoundException(request.Id);
		//var dto = (PostCompleteDataDto?)res;
		string HttpPath = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host.Value}";
		resDto.Attachments = resDto.Attachments.Select(a => AttachmentDto.Map(a, HttpPath)).ToList();
		return resDto;
	}
}
