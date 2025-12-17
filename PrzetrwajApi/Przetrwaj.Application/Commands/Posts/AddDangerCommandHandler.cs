using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos.Posts;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Commands.Posts;

public class AddDangerCommandHandler : ICommandHandler<AddDangerInternallCommand, PostCompleteDataDto>
{
	private readonly IPostRepository _postRepository;

	public AddDangerCommandHandler(IPostRepository postRepository)
	{
		_postRepository = postRepository;
	}


	public Task<PostCompleteDataDto> Handle(AddDangerInternallCommand request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
