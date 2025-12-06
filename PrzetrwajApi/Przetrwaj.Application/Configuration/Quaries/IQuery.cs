using MediatR;

namespace Przetrwaj.Application.Configuration.Quaries;

public interface IQuery<out TResult> : IRequest<TResult>
{
}
