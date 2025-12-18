using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;
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
			.AsNoTracking()
			.Include(x => x.IdAutorNavigation)
			.Include(x => x.IdCategoryNavigation)
			.Include(x => x.IdRegionNavigation)
			.Include(x => x.Attachments)
			.Include(x => x.Comments)
			.Include(x => x.Votes)
			.FirstOrDefaultAsync(u => u.IdPost == idPost.ToLower(), cancellationToken);
		return post;
	}

	public async Task<IEnumerable<PostOverviewDto>> GetDangerByRegionAsync(int idRegion, CancellationToken cancellationToken = default)
	{
		var posts = await _context.PostsDangerRO
			//.Where(p => p.Active == true && p.Category == CategoryType.Danger) //Posts without the pre-filtered(mapping) to PostsDanger
			.Where(p => p.IdRegion == idRegion)
			.Select(p => new PostOverviewDto
			{
				Id = p.IdPost,
				Title = p.Title,
				DateCreated = p.DateCreated,
				// Map Navigations safely
				Category = p.IdCategoryNavigation != null ? new CategoryDto
				{
					IdCategory = p.IdCategoryNavigation.IdCategory,
					Name = p.IdCategoryNavigation.Name
				} : null,
				Region = p.IdRegionNavigation != null ? new RegionOnlyDto
				{
					IdRegion = p.IdRegionNavigation.IdRegion,
					Name = p.IdRegionNavigation.Name
				} : null,
				// --- VOTE CALCULATIONS (Executed on Database side) ---
				VotePositive = p.Votes.Count(v => v.IsUpvote),
				VoteNegative = p.Votes.Count(v => !v.IsUpvote),
				VoteSum = p.Votes.Count(),
				VoteRatio = (p.Votes.Count() > 0)
				? ((float)p.Votes.Count(v => v.IsUpvote) / p.Votes.Count() * 100)
				: 100
			})
		.ToListAsync(cancellationToken);
		return posts;
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
