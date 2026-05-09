using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Impediments;

namespace Przetrwaj.Application.Quaries.Impediments;

public class GetImpedimentQueryHandler : IQueryHandler<GetImpedimentQuery, Impediment>
{
	private readonly IImpedimentsRepository _repo;

	public GetImpedimentQueryHandler(IImpedimentsRepository repo)
	{
		_repo = repo;
	}

	public async Task<Impediment> Handle(GetImpedimentQuery request, CancellationToken cancellationToken)
	{
		if (!(request.Id >= 0 && request.Id <= 31)) throw new ImpedimentIdException($"{request.Id} is not in [0;31] range");
		var res = await _repo.GetByIdAsync(request.Id, cancellationToken);
		if (res is null) throw new ImpedimentIdException($"Impediment {request.Id} does not exist");
		return res;
	}
}
