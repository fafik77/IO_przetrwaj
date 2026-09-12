using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Exceptions;

public class UserNotFoundException(string id) : NotFoundException<AppUser>(id)
{
}
