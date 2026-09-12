using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Exceptions;

public class UserAlreadyExistsException(string identity) : AlreadyExistsException<AppUser>(identity)
{
}
