using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
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
		throw new NotImplementedException();
		//var regions = (List<Region>)request;
		//foreach (var region in regions)
		//{
		//	await _regionRepository.AddAsync(region, cancellationToken);
		//}
		//try
		//{
		//	await _unitOfWork.SaveChangesAsync(cancellationToken);  //this could throw
		//}
		//catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
		//{
		//	throw new BadUpdateCommand(ex.InnerException.Message);
		//}
		//return regions
		//	.Select(r => (RegionOnlyDto)r!)
		//	.ToList();
	}
}
