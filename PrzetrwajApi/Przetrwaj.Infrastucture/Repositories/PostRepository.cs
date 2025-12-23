using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Posts;
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


	public async Task<Post> AddAsync(Post item, CancellationToken cancellationToken = default)
	{
		await _context.Posts.AddAsync(item, cancellationToken);
		return item;
	}

	public async Task<Attachment> AddAttachmentAsync(Attachment attachment, CancellationToken cancellationToken = default)
	{
		await _context.Attachments.AddAsync(attachment, cancellationToken);
		return attachment;
	}

	public Task<UserComment> AddCommentAsync(UserComment comment, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken = default)
	{
		try
		{
			var res = await _context.Votes.AddAsync(vote, cancellationToken);
			return vote;
		}
		catch (Exception ex)
		{
			//this Exception (Microsoft.EntityFrameworkCore.DbUpdateException) might only be thrown when performing .SaveChangesAsync()
			throw new AlreadyVotedException("Already Voted");
		}
	}

	public Task<IEnumerable<PostOverviewDto>> GetAllAuthoredByAsync(string idAuthor, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<IEnumerable<Attachment>> GetAttachmentsROAsync(string idPost, CancellationToken cancellationToken = default)
	{
		var res = await _context.Attachments.AsNoTracking().Where(a => a.IdPost == idPost).ToListAsync(cancellationToken);
		return res;
	}

	public async Task<PostCompleteDataDto?> GetFullROPostByIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		idPost = idPost.ToLower();
		return await _context.Posts
		.AsNoTracking()
		.Where(u => u.IdPost == idPost)
		.Select(p => new PostCompleteDataDto
		{
			Id = p.IdPost,
			Title = p.Title,
			Description = p.Description,
			CategoryType = p.Category,
			Comments = p.Comments
			.OrderByDescending(x => x.DateCreated)  //sort by Date desc (newest first)
			.Select(c => (CommentDto)c)
			.ToList(),
			DateCreated = p.DateCreated,
			Region = (RegionOnlyDto?)p.IdRegionNavigation,
			Author = (UserGeneralDto?)p.IdAutorNavigation,
			//if CustomCategory, fill this data with {id=customId, Name=CustomName not "other/inne"}
			Category = p.CustomCategory.Length > 0 ? new CategoryDto
			{
				IdCategory = p.IdCategory,
				Type = p.IdCategoryNavigation.Type,
				Name = p.CustomCategory,
			}
			: (CategoryDto?)p.IdCategoryNavigation,

			// Fetch only the bool values
			VotePositive = p.Votes.Count(p => p.IsUpvote),
			VoteNegative = p.Votes.Count(p => !p.IsUpvote),
			VoteSum = p.Votes.Count(),
			// Map attachments using the URL logic
			Attachments = p.Attachments
			.OrderBy(x => x.OrderInList)    //sort by OrderInList asc
			.Select(a => new AttachmentDto
			{
				AlternateDescription = a.AlternateDescription,
				FileURL = $"/Attachments/{a.IdAttachment}.webp",
			}).ToList()
		})
		.FirstOrDefaultAsync(cancellationToken: cancellationToken);
	}
	public async Task<Post?> GetPostWithAttachmentsByIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		var post = await _context.Posts
			.Include(x => x.Attachments.OrderBy(a => a.OrderInList))    //sort by OrderInList asc
			.FirstOrDefaultAsync(u => u.IdPost == idPost, cancellationToken);
		return post;
	}
	public async Task<Post?> GetRWPostByIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		var post = await _context.Posts
			.FirstOrDefaultAsync(u => u.IdPost == idPost, cancellationToken);
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
				//// Map Navigations safely
				//Category = p.IdCategoryNavigation != null ? new CategoryDto
				//{
				//	IdCategory = p.IdCategoryNavigation.IdCategory,
				//	Name = p.IdCategoryNavigation.Name
				//} : null,
				//if CustomCategory, fill this data with {id=customId, Name=CustomName not "other/inne"}
				Category = p.CustomCategory.Length > 0 ? new CategoryDto
				{
					IdCategory = p.IdCategory,
					Type = p.IdCategoryNavigation.Type,
					Name = p.CustomCategory,
				}
				: (CategoryDto?)p.IdCategoryNavigation,
				Region = p.IdRegionNavigation != null ? new RegionOnlyDto
				{
					IdRegion = p.IdRegionNavigation.IdRegion,
					Name = p.IdRegionNavigation.Name
				} : null,
				// --- VOTE CALCULATIONS (Executed on Database side) ---
				VotePositive = p.Votes.Count(v => v.IsUpvote),
				VoteNegative = p.Votes.Count(v => !v.IsUpvote),
				VoteSum = p.Votes.Count(),
				//VoteRatio = (p.Votes.Count() > 0)
				//? ((float)p.Votes.Count(v => v.IsUpvote) / p.Votes.Count() * 100)
				//: 100
			})
		.ToListAsync(cancellationToken);
		return posts;
	}

	public async Task<IEnumerable<PostMinimalCategoryRegion>> GetPostsMinimalCategoryRegion(CancellationToken cancellationToken = default)
	{
		return await _context.PostMinimalViews.AsNoTracking().Where(p => p.Active == true).ToListAsync(cancellationToken);
	}

	public Task<IEnumerable<PostOverviewDto>> GetResourceByRegionAsync(int idRegion, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void Update(Post post, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

}
