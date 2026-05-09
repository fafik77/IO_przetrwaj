using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Users;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Application.Quaries.Posts;

public class GetAllAuthoredByQueryHandler : IQueryHandler<GetAllAuthoredByQuery, IEnumerable<PostOverviewDto>>
{
	private readonly IPostRepository _postRepository;
	private readonly IUserRepository _userRepository;

	public GetAllAuthoredByQueryHandler(IPostRepository postRepository, IUserRepository userRepository)
	{
		_postRepository = postRepository;
		_userRepository = userRepository;
	}

	public async Task<IEnumerable<PostOverviewDto>> Handle(GetAllAuthoredByQuery request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdAsync(request.AutorId, cancellationToken);
		if (user is null)
			throw new UserNotFoundException(request.AutorId.ToLower());
		var res = await _postRepository.GetAllAuthoredByAsync(request.AutorId, cancellationToken);
		return res;
	}
}
