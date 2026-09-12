using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Exceptions;

public class CategoryNotFoundException(int id) : NotFoundException<Category>(id)
{
}
