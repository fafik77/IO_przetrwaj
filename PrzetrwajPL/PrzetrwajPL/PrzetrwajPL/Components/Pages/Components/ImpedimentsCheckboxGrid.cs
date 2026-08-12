using Microsoft.AspNetCore.Components;
using Przetrwaj.CommonLibrary.Consts;

namespace PrzetrwajPL.Components.Pages.Components;
/// <summary>
/// This class extends the <see cref="CheckboxGrid"/> Component with ways to load `Impediments` from API
/// </summary>
public class ImpedimentsCheckboxGrid : CheckboxGrid
{
	[Inject]
	private IHttpClientFactory ClientFactory { get; set; } = default!;
	private bool loadedValues = false;

	protected async Task LoadValuesAsync()
	{
		// start fetching data from API
		var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
		var impedimentsResponse = await client.GetAsync("/Impediments");
		loadedValues = true;
		if (!impedimentsResponse.IsSuccessStatusCode)
			return;
		KeyLabelPairs = await impedimentsResponse.Content.ReadFromJsonAsync<Dictionary<int, string>>() ?? new();
		LoadKeyLabelPairs(KeyLabelPairs);

		StateHasChanged();
	}

	public async Task InitValuesAsync()
	{
		if (loadedValues) return;
		if (KeyLabelPairs != null && KeyLabelPairs.Count != 0)
		{
			//values are already loaded use them
			LoadKeyLabelPairs(KeyLabelPairs);
			loadedValues = true;
			return;
		}
		await LoadValuesAsync();
	}

	public async Task InitValuesAsync(Dictionary<int, string> KeyLabelPairs)
	{
		if (KeyLabelPairs != null && KeyLabelPairs.Count != 0)
		{
			//values are already loaded use them
			LoadKeyLabelPairs(KeyLabelPairs);
			loadedValues = true;
			return;
		}
		//fallback
		await LoadValuesAsync();
	}

	public async Task LoadFromIntBitFieldAsync(int bits)
	{
		await InitValuesAsync();
		LoadFromIntBitField(bits);
	}
}