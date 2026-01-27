using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Exceptions.RegionException;

namespace Przetrwaj.Application.Commands.Regions;

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
		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
	}
}
