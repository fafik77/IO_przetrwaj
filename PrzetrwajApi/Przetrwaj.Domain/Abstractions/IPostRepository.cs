using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Domain.Abstractions;

public interface IPostRepository
{
	#region Get
	Task AddAsync(Post item, CancellationToken cancellationToken = default);
	public Task<Post?> GetByIdAsync(string idPost, CancellationToken cancellationToken = default);
	public Task<IEnumerable<PostOverviewDto>> GetDangerByRegionAsync(int idRegion, CancellationToken cancellationToken = default);
	public Task<IEnumerable<PostOverviewDto>> GetResourceByRegionAsync(int idRegion, CancellationToken cancellationToken = default);
	public Task<IEnumerable<PostOverviewDto>> GetAllAuthoredByAsync(string idAuthor, CancellationToken cancellationToken = default);
	public Task<IEnumerable<Attachment>> GetAttachmentsAsync(string idPost, CancellationToken cancellationToken = default);
	#endregion //Get

	//public void AddComment(string id, CancellationToken cancellationToken = default);
	//public void AddVote(string id, CancellationToken cancellationToken = default);

	public Task<Attachment> AddAttachmentAsync(Attachment attachment, CancellationToken cancellationToken = default);
	public Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken = default);
	public Task<UserComment> AddCommentAsync(UserComment comment, CancellationToken cancellationToken = default);
	public void MarkAsInactive(string id, CancellationToken cancellationToken = default);
}
