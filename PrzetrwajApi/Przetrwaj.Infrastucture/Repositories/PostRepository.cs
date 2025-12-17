using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

internal class PostRepository : IPostRepository
{
	private readonly ApplicationDbContext _context;

	public PostRepository(ApplicationDbContext context)
	{
		_context = context;
	}


	public Task AddAsync(Post item, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<Attachment> AddAttachmentAsync(Attachment attachment, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<UserComment> AddCommentAsync(UserComment comment, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<PostOverviewDto>> GetAllAuthoredByAsync(string idAuthor, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<Attachment>> GetAttachmentsAsync(string idPost, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<Post?> GetByIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<PostOverviewDto>> GetDangerByRegionAsync(int idRegion, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<PostOverviewDto>> GetResourceByRegionAsync(int idRegion, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void MarkAsInactive(string id, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
