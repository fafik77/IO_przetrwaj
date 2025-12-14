using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
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

	public void Delete(Post item)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<Post?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void Update(Post item)
	{
		throw new NotImplementedException();
	}
}
