using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Exceptions;

internal class CategoryAlreadyExistsException(string identity) : AlreadyExistsException<Category>(identity)
{
}
