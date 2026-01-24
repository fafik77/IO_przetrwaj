using Microsoft.AspNetCore.Components;
using PrzetrwajPL.Models;

namespace PrzetrwajPL.Components.Pages.Components;

public partial class RegionPicker
{
	// Two-way binding for ID
	[Parameter] public int SelectedRegionId { get; set; } = -1;
	[Parameter] public EventCallback<int> SelectedRegionIdChanged { get; set; }

	// Display Name
	public string SelectedRegionName { get; private set; } = "Wybierz Region";

	private bool isDropdownOpen = false;
	private List<RegionOnlyDto>? regions;

	protected override async Task OnInitializedAsync()
	{
		//moved to OnAfterRenderAsync
		SelectedRegionName = "Ładowanie regionów";
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			try
			{
				var client = ClientFactory.CreateClient("ServerAPI");
				regions = await client.GetFromJsonAsync<List<RegionOnlyDto>>("/Region");
				SelectedRegionName = "Wybierz Region";
				// If a default ID was passed from parent, find its name immediately
				if (SelectedRegionId != -1)
				{
					await PickRegion(SelectedRegionId);
				}
			}
			catch (Exception)
			{
				SelectedRegionName = "Błąd połączenia";
			}
			finally
			{
				StateHasChanged();
			}
		}
	}

	private void ToggleDropdown() => isDropdownOpen = !isDropdownOpen;
	private async Task SelectRegion(int id, string name) => await PickRegion(id);

	// PUBLIC METHOD: Allows the parent to trigger a selection manually
	public async Task PickRegion(int id)
	{
		if (regions == null)
		{
			SelectedRegionId = id;
			return;
		}

		var region = regions.FirstOrDefault(r => r.Id == id);
		if (region != null)
		{
			SelectedRegionName = region.Name;
			SelectedRegionId = id;
			isDropdownOpen = false;
			await SelectedRegionIdChanged.InvokeAsync(id);
			StateHasChanged(); // Force UI update
		}
	}
}