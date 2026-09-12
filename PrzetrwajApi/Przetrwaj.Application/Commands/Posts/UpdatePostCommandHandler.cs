using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;

namespace Przetrwaj.Application.Commands.Posts;

public class UpdatePostCommandHandler(ICurrentUserService currentUserService) : ICommandHandler<UpdatePostInternalCommand>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IPostRepository _postRepository;

    public async Task Handle(UpdatePostInternalCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");
        var post = await _postRepository.GetRWPostByIdAsync(request.Id, cancellationToken)
            ?? throw new PostNotFoundException(request.Id);
    }
}