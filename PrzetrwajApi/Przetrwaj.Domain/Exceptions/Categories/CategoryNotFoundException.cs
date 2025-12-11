using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions.Categories;

public class CategoryNotFoundException(int id) : NotFoundException<Category>(id)
{
}
