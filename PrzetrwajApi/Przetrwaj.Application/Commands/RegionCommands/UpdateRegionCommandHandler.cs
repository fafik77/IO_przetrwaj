using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions.RegionException;

namespace Przetrwaj.Application.Commands.RegionCommands;

public class UpdateRegionCommandHandler : ICommandHandler<UpdateRegionCommand>
{
	private readonly IRegionRepository _regionRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateRegionCommandHandler(IRegionRepository regionRepository, IUnitOfWork unitOfWork)
	{
		_regionRepository = regionRepository;
		_unitOfWork = unitOfWork;
	}


	public async Task Handle(UpdateRegionCommand request, CancellationToken cancellationToken)
	{
		var regionToUpdate = await _regionRepository.GetByIdAsync(request.IdRegion);
		if (regionToUpdate == null) throw new RegionNotFoundException(request.IdRegion);
		
		regionToUpdate.Name = request.Name;
		
		_regionRepository.Update(regionToUpdate);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
	}
}
