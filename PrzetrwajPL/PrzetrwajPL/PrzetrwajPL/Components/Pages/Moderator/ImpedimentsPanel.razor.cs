using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Requests;

namespace PrzetrwajPL.Components.Pages.Moderator;

public partial class ImpedimentsPanel
{
	private const string ToReachEndpoint = "/Impediments";
	private readonly Dictionary<short, string> impediments = new();
	private short? currentSelection;
	private string currentSelectionName = string.Empty;

	private bool isLoading = false;

	private string successMessage = string.Empty;
	private string errorMessage = string.Empty;


	protected override async Task OnInitializedAsync()
	{
		await LoadCategoriesList();
	}

	private async Task LoadCategoriesList()
	{
		impediments.Clear();
		//populate with 32 items
		for (short id = 0; id <= 31; ++id)
			impediments.Add(id, "");

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var result = await client.GetFromJsonAsync<Dictionary<short, string>>(ToReachEndpoint);
			if (result != null)
				foreach (var item in result)
				{
					impediments[item.Key] = item.Value;
				}
		}
		catch (Exception ex)
		{
			errorMessage = $"Nie uda�o si� wczyta� listy kategorii: {ex.Message}";
		}
	}

	private void SelectItemForEdit(short selected)
	{
		ClearMessages();
		currentSelection = selected;
		currentSelectionName = impediments[selected];
		StateHasChanged();
	}

	private async Task HandleSave()
	{
		if (currentSelection == null) return;
		if (string.IsNullOrWhiteSpace(currentSelectionName))
		{
			errorMessage = "Nazwa typu nie mo�e by� pusta.";
			return;
		}

		ClearMessages();
		isLoading = true;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			HttpResponseMessage response;

			//(update existing)/add impediment
			var cmd = new EditImpediment { Id = currentSelection.Value, Name = currentSelectionName };
			response = await client.PutAsJsonAsync(ToReachEndpoint, cmd);


			if (response.IsSuccessStatusCode)
			{
				successMessage = "Zmiany zosta�y zapisane pomy�lnie.";
				currentSelection = null; // deselect item and free the form
				await LoadCategoriesList(); // refresh Master-List
			}
			else
			{
				var errorResult = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
				errorMessage = errorResult?.Error?.Message ?? "Wyst�pi� b��d podczas zapisywania typu.";
			}
		}
		catch (Exception ex)
		{
			errorMessage = $"B��d komunikacji z API: {ex.Message}";
		}
		finally
		{
			isLoading = false;
		}
	}

	private void ClearMessages()
	{
		successMessage = string.Empty;
		errorMessage = string.Empty;
	}
}