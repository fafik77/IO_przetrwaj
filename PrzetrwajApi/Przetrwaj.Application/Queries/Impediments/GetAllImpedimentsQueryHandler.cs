using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Application.Quaries.Impediments;

public class GetAllImpedimentsQueryHandler : IQueryHandler<GetAllImpedimentsQuery,IEnumerable<Impediment>> {
	private readonly IImpedimentsRepository _repo;

	public GetAllImpedimentsQueryHandler(IImpedimentsRepository repo)
	{
		_repo = repo;
	}

	public async Task<IEnumerable<Impediment>> Handle(GetAllImpedimentsQuery request, CancellationToken cancellationToken)
	{
		return await _repo.GetAllAsync(cancellationToken);
	}
}
