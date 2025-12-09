using FluentValidation;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Application.Dtos;
using Przetrwaj.Domain.Abstractions;

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
		if (string.IsNullOrEmpty(request.RegionName)) throw new ValidationException("Invalid command: RegionName can not be null or empty");

		var region = new Domain.Entities.Region { Name = request.RegionName };
		await _regionRepository.AddAsync(region, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return (RegionOnlyDto)region;
	}
}
