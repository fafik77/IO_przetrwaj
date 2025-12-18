using Microsoft.EntityFrameworkCore;
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


	public async Task AddAsync(Post item, CancellationToken cancellationToken = default)
	{
		await _context.Posts.AddAsync(item, cancellationToken);
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

	public async Task<Post?> GetByIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		var post = await _context.Posts
			.Include(x => x.IdAutorNavigation)
			.Include(x => x.IdCategoryNavigation)
			.Include(x => x.IdRegionNavigation)
			.Include(x => x.Attachments)
			.Include(x => x.Comments)
			.Include(x => x.Votes)
			.FirstOrDefaultAsync(u => u.IdPost == idPost.ToLower(), cancellationToken);
		return post;
	}

	public Task<IEnumerable<PostOverviewDto>> GetDangerByRegionAsync(int idRegion, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<IEnumerable<PostMinimalCategoryRegionDto>> GetPostsMinimalCategoryRegion(CancellationToken cancellationToken = default)
	{
		return await _context.PostMinimalViews.ToListAsync(cancellationToken);
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
