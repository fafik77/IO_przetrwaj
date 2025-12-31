using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Domain.Abstractions;

public interface IPostRepository
{
	#region Get

	Task<Post> AddAsync(Post item, CancellationToken cancellationToken = default);
	/// <summary>
	/// Retrieves a read only post by its unique identifier with all 1st level props filled.
	/// </summary>
	/// <param name="idPost">The unique identifier of the post to retrieve. Cannot be null or empty.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the post with the specified identifier</returns>
	public Task<PostCompleteDataDto?> GetFullROPostByIdAsync(string idPost, CancellationToken cancellationToken = default);
	public Task<bool> ExistsPostIdAsync(string idPost, CancellationToken cancellationToken = default);
	public Task<Post?> GetRWPostByIdAsync(string idPost, CancellationToken cancellationToken = default);
	public Task<Post?> GetPostWithAttachmentsByIdAsync(string idPost, CancellationToken cancellationToken = default);
	public Task<IEnumerable<PostOverviewDto>> GetDangerByRegionAsync(int idRegion, CancellationToken cancellationToken = default);
	public Task<IEnumerable<PostOverviewDto>> GetResourceByRegionAsync(int idRegion, CancellationToken cancellationToken = default);
	public Task<IEnumerable<PostOverviewDto>> GetAllAuthoredByAsync(string idAuthor, CancellationToken cancellationToken = default);
	public Task<IEnumerable<Attachment>> GetAttachmentsROAsync(string idPost, CancellationToken cancellationToken = default);
	public Task<IEnumerable<PostMinimalCategoryRegion>> GetPostsMinimalCategoryRegion(CancellationToken cancellationToken = default);
	public Task<bool> PostExistsByIdAsync(string idPost, CancellationToken cancellationToken = default);
	#endregion //Get

	//public void AddComment(string id, CancellationToken cancellationToken = default);
	//public void AddVote(string id, CancellationToken cancellationToken = default);
	
	public Task<Attachment> AddAttachmentAsync(Attachment attachment, CancellationToken cancellationToken = default);
	public Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken = default);
	public Task<UserComment> AddCommentAsync(UserComment comment, CancellationToken cancellationToken = default);
	public void Update(Post post, CancellationToken cancellationToken = default);
	Task<Vote?> GetVoteAsync(string idPost, string idUser, CancellationToken cancellationToken = default);

}
