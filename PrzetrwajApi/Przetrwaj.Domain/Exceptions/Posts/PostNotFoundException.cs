using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Exceptions;

public class PostNotFoundException(string id) : NotFoundException<Post>(id)
{
}
