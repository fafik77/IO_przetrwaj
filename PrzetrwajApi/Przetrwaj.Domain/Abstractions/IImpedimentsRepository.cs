using Przetrwaj.Domain.Abstractions._base;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Abstractions;

public interface IImpedimentsRepository : IGetsAsyncRepository<Impediment>, ICUDAsyncRepository<Impediment>
{

}
