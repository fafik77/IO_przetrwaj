using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using PrzetrwajPL.Models;

namespace PrzetrwajPL.Components.Pages.Components;

public partial class RegionPicker
{
	[Inject] private IJSRuntime JsRuntime { get; set; } = default!;

	// Two-way binding for final selected ID (Gmina)
	[Parameter] public int SelectedRegionId { get; set; } = -1;
	[Parameter] public EventCallback<int> SelectedRegionIdChanged { get; set; }

	public string SelectedRegionName { get; private set; } = "Wybierz Region";
	private bool isGeolocationLoading = false;

	// Hierarchical Lists
	private List<RegionOnlyDto>? wojewodztwa;
	private List<RegionOnlyDto>? powiaty;
	private List<RegionOnlyDto>? gminy;

	// Active Selections
	private int? selectedWojId;
	private int? selectedPowId;
	private int? selectedGmiId;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			// If an initial ID was provided, fetch the full multi-layer path up to the root
			if (SelectedRegionId != -1)
			{
				await LoadRegionFromId(SelectedRegionId);
			}
			else
			{
				await LoadWojewodztwa();
			}
		}
	}

	private async Task LoadWojewodztwa()
	{
		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			wojewodztwa = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.WOJ}");
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
			selectedWojId = wojId;
			selectedPowId = null;
			selectedGmiId = null;
			gminy = null;
			powiaty = null;

			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			powiaty = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.POW}&ParentId={wojId}");
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
			selectedPowId = powId;
			selectedGmiId = null;
			gminy = null;

			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			gminy = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.GMI}&ParentId={powId}");
			if (gminy.Count == 1)
			{
				selectedGmiId = gminy[0].Id;
			}
		}
		else
		{
			selectedPowId = null;
			selectedGmiId = null;
			gminy = null;
		}
		StateHasChanged();
	}

	private async Task HandleGmiSelect(ChangeEventArgs e)
	{
		if (int.TryParse(e.Value?.ToString(), out var gmiId) && gmiId > 0)
		{
			selectedGmiId = gmiId;
			SelectedRegionId = gmiId;

			var gmiItem = gminy?.FirstOrDefault(g => g.Id == gmiId);
			if (gmiItem != null) SelectedRegionName = gmiItem.Name;

			await SelectedRegionIdChanged.InvokeAsync(SelectedRegionId);
		}
		else
		{
			selectedGmiId = null;
		}
	}

	public async Task LoadRegionFromId(int id)
	{
		if (id <= 0) return;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			int currentId = id;

			selectedGmiId = null;
			selectedPowId = null;
			selectedWojId = null;
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
					selectedGmiId = region.Id;
					currentId = region.ParentId;
				}
				else if (region.Type == RegionPrecision.POW)
				{
					selectedPowId = region.Id;
					currentId = region.ParentId;
				}
				else if (region.Type == RegionPrecision.WOJ)
				{
					selectedWojId = region.Id;
					currentId = 0; // Top reached, force loop exit
				}
				else
				{
					break;
				}
			}

			// Synchronously load cascading sibling select vectors based on resolved layout paths
			await LoadWojewodztwa();

			if (selectedWojId.HasValue)
			{
				powiaty = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.POW}&ParentId={selectedWojId}");
			}

			if (selectedPowId.HasValue)
			{
				gminy = await client.GetFromJsonAsync<List<RegionOnlyDto>>($"/Regions?precision={RegionPrecision.GMI}&ParentId={selectedPowId}");
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
		selectedWojId = null;
		selectedPowId = null;
		selectedGmiId = null;
		powiaty = null;
		gminy = null;
	}

	private async Task ResetToId(int id)
	{
		await LoadRegionFromId(id);
	}

	private class GeolocationCoords
	{
		public double Lat { get; set; }
		public double Lng { get; set; }
	}
}