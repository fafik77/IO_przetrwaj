using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions.Categories;

internal class CategoryAlreadyExistsException(string identity) : AlreadyExistsException<Category>(identity)
{
}
