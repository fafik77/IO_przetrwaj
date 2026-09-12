using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Extensions;

namespace Przetrwaj.Application.Commands.Posts;

public class UpdatePostCommandHandler(ICurrentUserService currentUserService, IPostRepository postRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdatePostInternalCommand>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IPostRepository _postRepository = postRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(UpdatePostInternalCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");
        var post = await _postRepository.GetRWPostByIdAsync(request.Id, cancellationToken)
            ?? throw new PostNotFoundException(request.Id);
        if (!post.IdAutor.Equals(userId, StringComparison.OrdinalIgnoreCase))
            throw new NotTheAuthorException($"not the author of {request.Id}");

        var model = request.UpdatePost;
        if (!model.Title.IsNullOrWhiteSpace())
            post.Title = model.Title!;

        if (!model.Description.IsNullOrWhiteSpace())
            post.Description = model.Description!;

        if (!model.CustomCategory.IsNullOrWhiteSpace() && post.CustomCategory.Length > 0)
            post.CustomCategory = model.CustomCategory!;

        _postRepository.Update(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}