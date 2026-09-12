using Przetrwaj.Application.Common.Interfaces;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Posts;

public class AddCommentCommandHandler : ICommandHandler<AddCommentInternalCommand, CommentDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddCommentCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<CommentDto> Handle(AddCommentInternalCommand request, CancellationToken cancellationToken)
    {
        if (!await _postRepository.ExistsActivePostIdAsync(request.IdPost, cancellationToken))
            throw new PostNotFoundException(request.IdPost);
        var userId = _currentUserService.UserId
            ?? throw new InvalidAuthorizationException("Not Authorized");
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var comment = new UserComment
        {
            IdAutor = userId,
            IdPost = request.IdPost,
            Comment = request.Comment,
        };
        var res = await _postRepository.AddCommentAsync(comment, cancellationToken);
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            throw new BadUpdateCommand(ex.InnerException.Message);
        }
        var dto = CommentDto.Map(res);
        dto.Author = UserGeneralDtoNoRegion.Map(user);
        return dto;
    }
}
