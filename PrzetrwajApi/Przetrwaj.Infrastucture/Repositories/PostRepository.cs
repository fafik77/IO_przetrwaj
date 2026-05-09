using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Posts;
using Przetrwaj.Domain.Helpers;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Domain.Models.Dtos.Posts;
using Przetrwaj.Infrastucture.Context;

namespace Przetrwaj.Infrastucture.Repositories;

internal class PostRepository : IPostRepository
{
	private readonly ApplicationDbContext _context;
	private readonly IRegionRepository _regionRepository;

	public PostRepository(ApplicationDbContext context, IRegionRepository regionRepository)
	{
		_context = context;
		_regionRepository = regionRepository;
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

	public async Task<UserComment> AddCommentAsync(UserComment comment, CancellationToken cancellationToken = default)
	{
		await _context.Comments.AddAsync(comment, cancellationToken);
		return comment;
	}

	public async Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken = default)
	{
		try
		{
			var res = await _context.Votes.AddAsync(vote, cancellationToken);
			return vote;
		}
		catch (Exception)
		{
			//this Exception (Microsoft.EntityFrameworkCore.DbUpdateException) might only be thrown when performing .SaveChangesAsync()
			throw new AlreadyVotedException("Already Voted");
		}
	}

	public async Task<IEnumerable<PostOverviewDto>> GetAllAuthoredByAsync(string idAuthor, CancellationToken cancellationToken = default)
	{
		var posts = await _context.Posts
			.Where(p => p.Active == true && p.IdAutor == idAuthor.ToLower())
			.Select(p => SelectAsPostOverview(p))
			.ToListAsync(cancellationToken);
		return posts;
	}

	public async Task<PostCompleteDataDto?> GetFullROPostByIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		idPost = idPost.ToLower();
		var res = await _context.Posts
		.AsNoTracking()
		.Where(u => u.IdPost == idPost)
		.Select(p => new PostCompleteDataDto
		{
			Id = p.IdPost,
			Title = p.Title,
			Description = p.Description,
			CategoryType = p.CategoryType,
			Comments = p.Comments
			.OrderByDescending(x => x.DateCreated)
			.Select(c => new CommentDto
			{
				Comment = c.Comment,
				DateCreated = c.DateCreated,
				Autor = c.IdAutorNavigation != null ? new UserGeneralDtoSimpleRegion
				{
					Id = c.IdAutorNavigation.Id,
					Name = c.IdAutorNavigation.Name ?? "",
					Surname = c.IdAutorNavigation.Surname ?? "",
					IdRegion = c.IdAutorNavigation.PowiatId ?? 0,
					RegistrationDate = c.IdAutorNavigation.RegistrationDate,
					BanDate = c.IdAutorNavigation.BanDate,
				} : null
			})
			.ToList(),
			DateCreated = p.DateCreated,
			// we have to re-map the region (as this is on DB side) later in the code
			Region = new RegionOnlyDto
			{
				Id = p.IdGmiOnly ?? p.IdPowOnly ?? p.IdWojOnly ?? 0,
				Name = string.Empty,
			},
			Author = (UserGeneralDtoSimpleRegion?)p.IdAutorNavigation,
			// if CustomCategory, fill this data with {id=customId, Name=CustomName not "other/inne"}
			Category = p.CustomCategory.Length > 0 ? new CategoryDto
			{
				Id = p.IdCategory,
				Type = p.IdCategoryNavigation.Type,
				Name = p.CustomCategory,
			}
			: (CategoryDto?)p.IdCategoryNavigation,

			// Fetch only the bool values
			VotePositive = p.Votes.LongCount(p => p.IsUpvote),
			VoteNegative = p.Votes.LongCount(p => !p.IsUpvote),
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
		if (res is null) return null;
		res.Region = RegionOnlyDto.Map(await _regionRepository.GetByIdAsync(res.Region?.Id ?? 0, cancellationToken));
		return res;
	}
	public async Task<Post?> GetPostWithAttachmentsByIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		idPost = idPost.ToLower();
		var post = await _context.Posts
			.Include(x => x.Attachments.OrderBy(a => a.OrderInList))    //sort by OrderInList asc
			.FirstOrDefaultAsync(u => u.IdPost == idPost, cancellationToken);
		return post;
	}
	public async Task<Post?> GetRWPostByIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		idPost = idPost.ToLower();
		var post = await _context.Posts
			.FirstOrDefaultAsync(u => u.IdPost == idPost, cancellationToken);
		return post;
	}
	public async Task<IEnumerable<PostMinimalCategoryRegion>> GetPostsMinimalCategoryRegion(CancellationToken cancellationToken = default)
	{
		return await _context.PostMinimalViews.AsNoTracking().Where(p => p.Active == true).ToListAsync(cancellationToken);
	}


	private async Task<IEnumerable<PostOverviewDto>> FillInPostDataAfterFetch(IEnumerable<PostOverviewDto> posts, CancellationToken cancellationToken)
	{
		foreach (var post in posts)
		{
			post.Region = RegionOnlyDto.Map(await _regionRepository.GetByIdAsync(post.Region?.Id ?? 0, cancellationToken));
			var votes = post.VotePositive + post.VoteNegative;
			post.VoteRatio = (votes > 0)
				? ((float)post.VotePositive / votes * 100)
				: 100;
		}
		return posts;
	}

	/// <summary>
	/// This function is the Func(in, out) of _context.Select(), has to follow the code to SQL rules
	/// </summary>
	/// <param name="p">the post on which DB is running Select</param>
	/// <returns>PostOverviewDto</returns>
	private static PostOverviewDto SelectAsPostOverview(Post p)
	{
		return new PostOverviewDto
		{
			Id = p.IdPost,
			Title = p.Title,
			DateCreated = p.DateCreated,
			Category = p.CustomCategory.Length > 0 ? new CategoryDto
			{
				Id = p.IdCategory,
				Type = p.IdCategoryNavigation.Type,
				Name = p.CustomCategory,
			}
			: (CategoryDto?)p.IdCategoryNavigation,
			Region = new RegionOnlyDto
			{
				Id = p.IdGmiOnly ?? p.IdPowOnly ?? p.IdWojOnly ?? 0,
				Name = string.Empty,
			},
			// --- VOTE CALCULATIONS (Executed on Database side) ---
			VotePositive = p.Votes.LongCount(v => v.IsUpvote),
			VoteNegative = p.Votes.LongCount(v => !v.IsUpvote),
		};
	}

	public async Task<IEnumerable<PostOverviewDto>> GetDangerByRegionAsync(int idRegion, CancellationToken cancellationToken = default)
	{
		var (Woj, Pow, Gmi) = RegionCompoundHelper.RegionSplit(idRegion);

		var posts = await _context.PostsDangerRO
			.Where(p => p.IdGmiOnly == Gmi || p.IdPowOnly == Pow || p.IdWojOnly == Woj || p.IdWojOnly == 0)
			.OrderByDescending(k => k.DateCreated)
			.Select(p => SelectAsPostOverview(p))
			.ToListAsync(cancellationToken);
		return await FillInPostDataAfterFetch(posts, cancellationToken);
	}
	public async Task<IEnumerable<PostOverviewDto>> GetResourceByRegionAsync(int idRegion, CancellationToken cancellationToken = default)
	{
		var (Woj, Pow, Gmi) = RegionCompoundHelper.RegionSplit(idRegion);

		var posts = await _context.PostsResourcesRO
			.Where(p => p.IdGmiOnly == Gmi || p.IdPowOnly == Pow || p.IdWojOnly == Woj || p.IdWojOnly == 0)
			.Select(p => SelectAsPostOverview(p))
			.ToListAsync(cancellationToken);
		return await FillInPostDataAfterFetch(posts, cancellationToken);
	}
	/// <summary>
	/// The new optimal metod to get Posts
	/// </summary>
	/// <param name="filter"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	public async Task<IEnumerable<PostOverviewDto>> GetMatchingPostsAsync(MatchingPostsFilter filter, CancellationToken ct = default)
	{
		var (Woj, Pow, Gmi) = RegionCompoundHelper.RegionSplit(filter.RegionId);
		CategoryType? type = filter.CategoryFilter switch
		{
			CategoryTypeFilter.Danger => CategoryType.Danger,
			CategoryTypeFilter.Resource => CategoryType.Resource,
			_ => null
		};

		var posts = await _context.Posts
			.AsNoTracking()
			.Where(p => p.Active == true && (type == null || p.CategoryType == type))
			.Where(p => p.IdGmiOnly == Gmi || p.IdPowOnly == Pow || p.IdWojOnly == Woj || p.IdWojOnly == 0)
			.OrderByDescending(p => p.DateCreated)
			.Select(p => SelectAsPostOverview(p))
			.ToListAsync(ct);
		return await FillInPostDataAfterFetch(posts, ct);
	}


	public void Update(Post post, CancellationToken cancellationToken = default)
	{
		_context.Posts.Update(post);
	}

	public async Task<Vote?> GetVoteAsync(string idPost, string idUser, CancellationToken cancellationToken = default)
	{
		idPost = idPost.ToLower();
		idUser = idUser.ToLower();
		return await _context.Votes
			.AsNoTracking()
			.FirstOrDefaultAsync(v => v.IdPost == idPost && v.IdUser == idUser, cancellationToken);
	}

	public async Task<bool> ExistsPostIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		idPost = idPost.ToLower();
		return await _context.Posts.AsNoTracking().Where(p => p.IdPost == idPost).AnyAsync(cancellationToken);
	}

	public async Task<bool> ExistsActivePostIdAsync(string idPost, CancellationToken cancellationToken = default)
	{
		idPost = idPost.ToLower();
		return await _context.Posts.AsNoTracking().Where(p => p.IdPost == idPost && p.Active == true).AnyAsync(cancellationToken);
	}

	public async Task<int> ArchiveInactivePostsAsync(CancellationToken ct = default)
	{
		// This performs the entire filter AND update in a single SQL query
		int affectedRows = await _context.Posts
			.Where(p => p.Active && p.Votes.LongCount(v => !v.IsUpvote) > p.Votes.LongCount(v => v.IsUpvote))
			.ExecuteUpdateAsync(s => s.SetProperty(p => p.Active, false), ct);
		return affectedRows;
	}
}
