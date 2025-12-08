using System.Net;

namespace Przetrwaj.Domain.Exceptions._base;

public class NotFoundException<T> : BaseException
{
	public int Id { get; set; }
	public string? IdString { get; set; }

	public NotFoundException(int id) : base($"{typeof(T).Name} id:{id} not found") => Id = id;
	public NotFoundException(string id) : base($"{typeof(T).Name} id:{id} not found") => IdString = id;

	public override HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;
}
