using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Application.Dtos.Posts;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Posts;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostCompleteDataDto>
{
	private readonly IPostRepository _postRepository;

	public GetPostByIdQueryHandler(IPostRepository postRepository)
	{
		_postRepository = postRepository;
	}


	public async Task<PostCompleteDataDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
	{
		var res = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
		if (res is null) throw new PostNotFoundException(request.Id);
		return (PostCompleteDataDto)res;
	}
}
