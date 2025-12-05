using System.Net;

namespace Przetrwaj.Domain.Exceptions._base;

public class AlreadyExistsException<T> : BaseException
{
	public string Identity { get; set; }
	public AlreadyExistsException(string identity) : base($"{typeof(T).Name} identified by: {identity} already exists") => Identity = identity;

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.Conflict;
}
