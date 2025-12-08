using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.RegionException;

namespace Przetrwaj.Application.Commands.RegionCommands;

public class DeleteRegionCommandHandler : ICommandHandler<DeleteRegionCommand>
{
	private readonly IRegionRepository _regionRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteRegionCommandHandler(IRegionRepository regionRepository, IUnitOfWork unitOfWork)
	{
		_regionRepository = regionRepository;
		_unitOfWork = unitOfWork;
	}


	public async Task Handle(DeleteRegionCommand request, CancellationToken cancellationToken)
	{
		var region = await _regionRepository.GetByIdAsync(request.RegionId, cancellationToken);
		if (region == null) throw new RegionNotFoundException(request.RegionId);
		_regionRepository.Delete(region);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
	}
}
