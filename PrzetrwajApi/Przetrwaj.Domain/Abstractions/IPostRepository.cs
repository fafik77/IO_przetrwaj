using Przetrwaj.Domain.Abstractions._base;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface IPostRepository : ICUDAsyncRepository<Post>, IGetsAsyncRepository<Post>
{

}
