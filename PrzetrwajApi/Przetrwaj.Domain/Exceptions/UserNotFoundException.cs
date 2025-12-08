using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions;

public class UserNotFoundException(string id) : NotFoundException<AppUser>(id)
{
}
