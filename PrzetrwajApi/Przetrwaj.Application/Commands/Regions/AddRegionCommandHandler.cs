using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Regions;

public class AddRegionCommandHandler : ICommandHandler<AddRegionCommand, RegionOnlyDto>
{
	private readonly IRegionRepository _regionRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AddRegionCommandHandler(IRegionRepository regionRepository, IUnitOfWork unitOfWork)
	{
		_regionRepository = regionRepository;
		_unitOfWork = unitOfWork;
	}


	public async Task<RegionOnlyDto> Handle(AddRegionCommand request, CancellationToken cancellationToken)
	{
		var region = (Region)request;
		await _regionRepository.AddAsync(region, cancellationToken);
		try
		{
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
		catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		{
			throw new BadUpdateCommand(ex.InnerException.Message);
		}
		return (RegionOnlyDto)region!;
	}
}
