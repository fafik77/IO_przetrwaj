using MediatR;

namespace Przetrwaj.Application.Configuration.Quaries;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
	where TQuery : IQuery<TResult>
{
}
