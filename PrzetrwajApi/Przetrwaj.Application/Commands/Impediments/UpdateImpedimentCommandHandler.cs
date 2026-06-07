using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions.Impediments;

namespace Przetrwaj.Application.Commands.Impediments;

public class UpdateImpedimentCommandHandler : ICommandHandler<UpdateImpedimentCommand, Impediment>
{
	private readonly IImpedimentsRepository _repo;
	private readonly IUnitOfWork _uow;

	public UpdateImpedimentCommandHandler(IImpedimentsRepository repo, IUnitOfWork uow)
	{
		_repo = repo;
		_uow = uow;
	}

	public async Task<Impediment> Handle(UpdateImpedimentCommand request, CancellationToken cancellationToken)
	{
		var item = request.Map();
		if (!(item.Id >= 0 && item.Id <= 31)) throw new ImpedimentIdException($"{item.Id} is not in [0;31] range");
		var exists = _repo.GetByIdAsync(item.Id, cancellationToken);
		if (exists != null)
			_repo.Update(item);
		else
			await _repo.AddAsync(item, cancellationToken);
		try
		{
			await _uow.SaveChangesAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new ImpedimentIdException($"Failed to save {item.Id}: {item.Name}");
		}
		return item;
	}
}
