using Przetrwaj.Application.Configuration.Quaries;
using Przetrwaj.Domain.Abstractions;

namespace Przetrwaj.Application.Queries.Impediments;

public class GetAllImpedimentsQueryHandler : IQueryHandler<GetAllImpedimentsQuery, IDictionary<short, string>>
{
	private readonly IImpedimentsRepository _repo;

	public GetAllImpedimentsQueryHandler(IImpedimentsRepository repo)
	{
		_repo = repo;
	}

	public async Task<IDictionary<short, string>> Handle(GetAllImpedimentsQuery request, CancellationToken cancellationToken)
	{
		return await _repo.GetAllAsync(cancellationToken);
	}
}
