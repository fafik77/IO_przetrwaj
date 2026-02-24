using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.Impediments;

namespace Przetrwaj.Application.Commands.Impediments;

public class DeleteImpedimentCommandHandler : ICommandHandler<DeleteImpedimentCommand>
{
	private readonly IImpedimentsRepository _repo;
	private readonly IUnitOfWork _uow;

	public DeleteImpedimentCommandHandler(IImpedimentsRepository repo, IUnitOfWork uow)
	{
		_repo = repo;
		_uow = uow;
	}

	public async Task Handle(DeleteImpedimentCommand request, CancellationToken cancellationToken)
	{
		if (!(request.Id >= 0 && request.Id <= 31)) throw new ImpedimentIdException($"{request.Id} is not in [0;31] range");
		var item = await _repo.GetByIdAsync(request.Id);
		if (item is null) return;
		_repo.Delete(item);
		await _uow.SaveChangesAsync(cancellationToken);
	}
}
