using Przetrwaj.Domain.Exceptions._base;
using System.Net;

namespace Przetrwaj.Domain.Exceptions.Users;

public class AccountUpdateException(string msg) : BaseException(msg)
{
	public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}

public class AccountPasswordUpdateException(string msg) : AccountUpdateException(msg)
{
}

public class AccountEmailUpdateException(string msg) : AccountUpdateException(msg)
{
}
