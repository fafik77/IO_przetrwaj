using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;

namespace PrzetrwajPL.Components.Pages.Components;

public partial class RegionPicker
{
	[Inject] private IJSRuntime JsRuntime { get; set; } = default!;

	/// <summary>
	/// Two-way binding for final selected ID (Gmina)
	/// </summary>
	[Parameter] public int SelectedRegionId { get; set; } = -1;
	[Parameter] public EventCallback<int> SelectedRegionIdChanged { get; set; }

	public static readonly string SelectRegionMsg = "Wybierz Region";
	public string SelectedRegionName { get; private set; } = SelectRegionMsg;
	private bool isGeolocationLoading = false;

	// Hierarchical Lists
	private List<RegionOnlyDto>? wojewodztwa;
	private List<RegionOnlyDto>? powiaty;
	private List<RegionOnlyDto>? gminy;

	// Active Selections
	public int? SelectedWojId { get; private set; }
	public int? SelectedPowId { get; private set; }
	public int? SelectedGmiId { get; private set; }
	public int? GetRegionId(RegionPrecision regionPrecision) => regionPrecision switch
	{
		RegionPrecision.PL => 0,
		RegionPrecision.WOJ => SelectedWojId,
		RegionPrecision.POW => SelectedPowId,
		RegionPrecision.GMI => SelectedGmiId,
	};

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			// If an initial ID was provided, fetch the full multi-layer path up to the root
			if (SelectedRegionId > 0)
			{
				await LoadRegionFromId(SelectedRegionId);
			}
			else
			{
				await LoadWojewodztwa();
			}
		}
	}

	private void SelectGmi(int? gmina = null, string? name = null)
	{
		if (gmina == null)
		{
			SelectedRegionId = -1;
			SelectedRegionName = SelectRegionMsg;
			return;
		}
		SelectedRegionId = gmina.Value;
		SelectedRegionName = name ?? "Miejscowość bez nazwy!";
	}

	private async Task LoadWojewodztwa()
	{
		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			wojewodztwa = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.WOJ}");
			wojewodztwa?.RemoveAll(r => r.Id == 0); //remove "Polska"
			wojewodztwa = wojewodztwa?.OrderBy(r => r.Name, Consts.PolishAlphabetComparer).ToList();
			StateHasChanged();
		}
		catch (Exception)
		{
			SelectedRegionName = "Błąd ładowania danych";
		}
	}

	private async Task HandleWojSelect(ChangeEventArgs e)
	{
		if (int.TryParse(e.Value?.ToString(), out var wojId) && wojId > 0)
		{
			SelectedWojId = wojId;
			SelectedPowId = null;
			SelectedGmiId = null;
			gminy = null;
			powiaty = null;

			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			powiaty = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.POW}&ParentId={wojId}");
			powiaty = powiaty?.OrderBy(r => r.Name, Consts.PolishAlphabetComparer).ToList();
			SelectGmi();
		}
		else
		{
			ResetSelections();
		}
		StateHasChanged();
	}

	private async Task HandlePowSelect(ChangeEventArgs e)
	{
		if (int.TryParse(e.Value?.ToString(), out var powId) && powId > 0)
		{
			SelectedPowId = powId;
			SelectedGmiId = null;
			gminy = null;

			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			gminy = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.GMI}&ParentId={powId}");
			gminy = gminy?.OrderBy(r => r.Name, Consts.PolishAlphabetComparer).ToList();
			if (gminy != null && gminy.Count == 1)
			{
				await SelectGminaById(gminy[0].Id);
			}
			else
				SelectGmi();
		}
		else
		{
			SelectedPowId = null;
			SelectedGmiId = null;
			gminy = null;
		}
		StateHasChanged();
	}

	private async Task HandleGmiSelect(ChangeEventArgs e)
	{
		if (int.TryParse(e.Value?.ToString(), out var gmiId) && gmiId > 0)
		{
			await SelectGminaById(gmiId);
		}
		else
		{
			SelectedGmiId = null;
		}
	}
	private async Task SelectGminaById(int gmiId)
	{
		SelectedGmiId = gmiId;

		var gmiItem = gminy?.FirstOrDefault(g => g.Id == gmiId);
		SelectGmi(gmiId, gmiItem?.Name);

		await SelectedRegionIdChanged.InvokeAsync(SelectedRegionId);
	}

	public async Task LoadRegionFromId(int id)
	{
		if (id <= 0) return;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			int currentId = id;

			SelectedGmiId = null;
			SelectedPowId = null;
			SelectedWojId = null;
			string? resolvedLeafName = null;

			// Traverse upwards until ParentId hits 0
			while (currentId > 0)
			{
				var region = await client.GetFromJsonAsync<RegionOnlyWithinDto>($"/Regions/{currentId}");
				if (region == null) break;

				// Keep the leaf node's name for display tracking optimization
				resolvedLeafName ??= region.Name;

				if (region.Type == RegionPrecision.GMI)
				{
					SelectedGmiId = region.Id;
					currentId = region.ParentId;
				}
				else if (region.Type == RegionPrecision.POW)
				{
					SelectedPowId = region.Id;
					currentId = region.ParentId;
				}
				else if (region.Type == RegionPrecision.WOJ)
				{
					SelectedWojId = region.Id;
					currentId = 0; // Top reached, force loop exit
				}
				else
				{
					break;
				}
			}

			// Synchronously load cascading sibling select vectors based on resolved layout paths
			await LoadWojewodztwa();

			if (SelectedWojId.HasValue)
			{
				powiaty = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.POW}&ParentId={SelectedWojId}");
			}

			if (SelectedPowId.HasValue)
			{
				gminy = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.GMI}&ParentId={SelectedPowId}");
			}

			if (!string.IsNullOrEmpty(resolvedLeafName))
			{
				SelectedRegionName = resolvedLeafName;
			}

			SelectedRegionId = id;
			await SelectedRegionIdChanged.InvokeAsync(SelectedRegionId);
			StateHasChanged();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Błąd podczas odtwarzania ścieżki regionu: {ex.Message}");
		}
	}

	private async Task RegionFromLocation()
	{
		isGeolocationLoading = true;
		StateHasChanged();

		try
		{
			var coords = await JsRuntime.InvokeAsync<GeolocationCoords>("eval", @"
				new Promise((resolve, reject) => {
					navigator.geolocation.getCurrentPosition(
						pos => resolve({ lat: pos.coords.latitude, lng: pos.coords.longitude }),
						err => reject(err)
					);
				})");

			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var payload = new { lat = coords.Lat, @long = coords.Lng };

			var response = await client.PostAsJsonAsync("/Regions/from-location", payload);

			if (response.IsSuccessStatusCode)
			{
				var resolvedRegion = await response.Content.ReadFromJsonAsync<RegionOnlyDto>();
				if (resolvedRegion != null)
				{
					// Populate all drops automatically from geolocation coordinates
					await LoadRegionFromId(resolvedRegion.Id);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Błąd geolokalizacji: {ex.Message}");
		}
		finally
		{
			isGeolocationLoading = false;
			StateHasChanged();
		}
	}

	private void ResetSelections()
	{
		SelectedWojId = null;
		SelectedPowId = null;
		SelectedGmiId = null;
		powiaty = null;
		gminy = null;
	}

	private class GeolocationCoords
	{
		public double Lat { get; set; }
		public double Lng { get; set; }
	}
}