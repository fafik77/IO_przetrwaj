using Microsoft.AspNetCore.Http;
using Przetrwaj.Application.Configuration.Commands;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Exceptions;
using Przetrwaj.Domain.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Przetrwaj.Application.Commands.Regions;

public class UpdateTercRegionsCommand : ICommand<UpdateTercRegionsResults>
{
	[Required]
	public required IFormFile File { get; set; }
}

public class UpdateTercRegionsCommandHandler : ICommandHandler<UpdateTercRegionsCommand, UpdateTercRegionsResults>
{
	private readonly IRegionRepository _regionRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateTercRegionsCommandHandler(IRegionRepository regionRepository, IUnitOfWork unitOfWork)
	{
		_regionRepository = regionRepository;
		_unitOfWork = unitOfWork;
	}


	public async Task<UpdateTercRegionsResults> Handle(UpdateTercRegionsCommand request, CancellationToken ct)
	{
		var results = new UpdateTercRegionsResults();
		if (!request.File.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase))
		{
			results.StatusCode = System.Net.HttpStatusCode.BadRequest;
			results.Success = false;
			results.Timestamp = DateTimeOffset.UtcNow;
			results.Error = new ErrorDetails
			{
				Code = nameof(NothingChangedException),
				Message = "InvalidFile"
			};
			return results;
		}

		using var fs = request.File.OpenReadStream();
		var parsedRegions = TercParser.Parse(fs);
		fs.Close();
		var regions = await _regionRepository.GetAllAsync(ct);
		var listNew = parsedRegions;
		var listCurrent = regions;

		var (res, count) = await Merge2ListsAsync(listNew.woj, listCurrent.Woj, RegionPrecision.WOJ, ct);
		results.Results.AddRange(res.Results);
		results.WojCount = count;

		(res, count) = await Merge2ListsAsync(listNew.pow, listCurrent.Pow, RegionPrecision.POW, ct);
		results.Results.AddRange(res.Results);
		results.PowCount = count;

		(res, count) = await Merge2ListsAsync(listNew.gmi, listCurrent.Gmi, RegionPrecision.GMI, ct);
		results.Results.AddRange(res.Results);
		results.GmiCount = count;

		try
		{
			await _unitOfWork.SaveChangesAsync(ct);
		}
		catch (Exception ex)
		{
			results.Error = new ErrorDetails { Code = ex.InnerException?.GetType().Name ?? "Database Error", Message = ex.Message };
			results.Success = false;
		}
		results.Timestamp = DateTimeOffset.Now;
		return results;
	}

	private async Task<(UpdateTercRegionsResults res, short count)> Merge2ListsAsync<T>(IList<T> listNew, IList<T> listCurrent, RegionPrecision regionType, CancellationToken ct) where T : class, IRegionInfo
	{
		var results = new UpdateTercRegionsResults();
		var comparer = new GenericCompare<T>(x => x.Id);
		int count = listCurrent.Count;
		var RegionsToAdd = listNew.Except(listCurrent, comparer: comparer).ToList();
		var RegionsToRemove = listCurrent.Except(listNew, comparer: comparer).Where(i => i.Id != 0).ToList();
		count += RegionsToAdd.Count() - RegionsToRemove.Count();

		results.Results.AddRange(RegionsToAdd.Select(i => new UpdateTercRegionResult
		{
			Type = regionType,
			Status = "Added",
			Region = i,
		}));
		results.Results.AddRange(RegionsToRemove.Select(i => new UpdateTercRegionResult
		{
			Type = regionType,
			Status = "Removed",
			Region = i,
		}));
		var updateItems = new List<T>();
		foreach (var item in listCurrent)
		{
			var compareTo = listNew.FirstOrDefault(i => i.Id == item.Id);
			if (compareTo is null) continue;
			if (!item.Name.Equals(compareTo.Name))
			{
				var was = item.Name;
				item.Name = compareTo.Name;
				updateItems.Add(item);
				results.Results.Add(new UpdateTercRegionResult
				{
					Type = regionType,
					Status = "Updated",
					Region = item,
					OldName = was,
				});
			}
		}

		await _regionRepository.AddAsync(RegionsToAdd, ct);
		_regionRepository.Delete(RegionsToRemove);
		_regionRepository.Update(updateItems);

		return (results, (short)count);
	}

}
