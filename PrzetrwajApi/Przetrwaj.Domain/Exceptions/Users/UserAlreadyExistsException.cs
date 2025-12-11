using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions._base;

namespace Przetrwaj.Domain.Exceptions.Users;

public class UserAlreadyExistsException(string identity) : AlreadyExistsException<AppUser>(identity)
{
}
