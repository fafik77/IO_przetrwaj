using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos;

namespace Przetrwaj.Application.Commands.Regions;

public class AddRegionsCommandHandler : ICommandHandler<AddRegionsCommand, IEnumerable<RegionOnlyDto>>
{
	private readonly IRegionRepository _regionRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AddRegionsCommandHandler(IRegionRepository regionRepository, IUnitOfWork unitOfWork)
	{
		_regionRepository = regionRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<IEnumerable<RegionOnlyDto>> Handle(AddRegionsCommand request, CancellationToken cancellationToken)
	{
		var regions = (List<Region>)request;
		regions.ForEach(
			async region => await _regionRepository.AddAsync(region, cancellationToken)
		);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return regions
			.Select(r => (RegionOnlyDto)r!)
			.ToList();
	}
}
