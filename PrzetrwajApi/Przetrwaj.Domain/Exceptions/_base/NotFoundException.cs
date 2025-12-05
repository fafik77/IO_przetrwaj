using System.Net;

namespace Przetrwaj.Domain.Exceptions._base;

public class NotFoundException<T> : BaseException
{
	public int Id { get; set; }

	public NotFoundException(int id) : base($"{typeof(T).Name} id:{id} not found") => Id = id;

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;
}
