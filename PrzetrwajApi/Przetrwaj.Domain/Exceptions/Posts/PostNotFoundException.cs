using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions.Posts;

public class PostNotFoundException(string id) : NotFoundException<Post>(id)
{
}
