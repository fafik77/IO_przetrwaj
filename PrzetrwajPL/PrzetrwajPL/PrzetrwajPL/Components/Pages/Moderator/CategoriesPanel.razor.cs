using Microsoft.AspNetCore.Components;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Requests;
using PrzetrwajPL.Components.Pages.Components;

namespace PrzetrwajPL.Components.Pages.Moderator;

public partial class CategoriesPanel
{
	[Parameter]
	public required string CategoriesType { get; set; } = "dangers";
	private string CategoriesToReachEndpoint => "/Categories/" + CategoriesType;
	private string CategoriesPlName => CategoriesType.Equals("dangers", StringComparison.OrdinalIgnoreCase) ? "zagro¿eñ" : "zasobów";
	private List<CategoryDto>? categories;
	private CategoryDto? currentCategory;

	private ImpedimentsCheckboxGrid? AppliesToCheckboxGrid;
	private bool isNewCategory = false;
	private bool isLoading = false;
	private bool isAppliesToCheckboxGridOpen = false;
	private void ToggleAppliesToCheckboxPicker() => isAppliesToCheckboxGridOpen = !isAppliesToCheckboxGridOpen;
	private Dictionary<int, string> KeyLabelPairsAppliesTo = new();

	private string successMessage = string.Empty;
	private string errorMessage = string.Empty;


	protected override async Task OnInitializedAsync()
	{
		await LoadCategoriesList();
	}

	private async Task LoadCategoriesList()
	{
		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var result = await client.GetFromJsonAsync<List<CategoryDto>>(CategoriesToReachEndpoint);
			if (result != null)
				categories = result;
		}
		catch (Exception ex)
		{
			errorMessage = $"Nie uda³o siê wczytaæ listy kategorii: {ex.Message}";
		}
	}

	private async Task InitiateNewCategoryAsync()
	{
		ClearMessages();
		isNewCategory = true;

		currentCategory = new CategoryDto
		{
			//id -1 is not valid but seting it is required
			Id = -1,
			Name = string.Empty,
			Impediments = 0
		};
		//the component does not exist, only after rendering it does exist
		StateHasChanged();
		await Task.Delay(1);
		//show impediments list
		if (AppliesToCheckboxGrid != null)
			await AppliesToCheckboxGrid.InitValuesAsync(KeyLabelPairsAppliesTo);

		//wait for Blazor to update page
		StateHasChanged();
	}

	private async Task SelectCategoryForEdit(CategoryDto selected)
	{
		ClearMessages();
		isNewCategory = false;
		// copy the properties of the selected object so we wont update the item in the list untill we hit save
		currentCategory = new CategoryDto
		{
			Id = selected.Id,
			Name = selected.Name,
			Type = selected.Type,
			Impediments = selected.Impediments
		};

		//the component does not exist, only after rendering it does exist
		StateHasChanged();
		await Task.Delay(1);

		// load Impediments
		if (AppliesToCheckboxGrid != null)
		{
			await AppliesToCheckboxGrid.InitValuesAsync(KeyLabelPairsAppliesTo);
			await AppliesToCheckboxGrid.LoadFromIntBitFieldAsync(selected.Impediments);
		}
		StateHasChanged();
	}

	private async Task HandleSave()
	{
		if (currentCategory == null) return;
		if (string.IsNullOrWhiteSpace(currentCategory.Name))
		{
			errorMessage = "Nazwa kategorii nie mo¿e byæ pusta.";
			return;
		}

		ClearMessages();
		isLoading = true;

		currentCategory.Impediments = AppliesToCheckboxGrid?.GetSelectedAsIntBitField() ?? 0;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			HttpResponseMessage response;
			int selectCategoryId;

			if (isNewCategory)
			{
				//make new category
				var cmd = new AddUpdateCategory { Impediments = currentCategory.Impediments, Name = currentCategory.Name };
				response = await client.PostAsJsonAsync(CategoriesToReachEndpoint, cmd);
				var category = response.Content.ReadFromJsonAsync<CategoryDto>();
				selectCategoryId = category.Id;
			}
			else
			{
				//update existing category
				var cmd = new AddUpdateCategory { Impediments = currentCategory.Impediments, Name = currentCategory.Name };
				response = await client.PutAsJsonAsync(CategoriesToReachEndpoint + "/" + currentCategory.Id, cmd);
				selectCategoryId = currentCategory.Id;
			}

			if (response.IsSuccessStatusCode)
			{
				successMessage = isNewCategory ? "Pomyœlnie dodano now¹ kategoriê." : "Zmiany zosta³y zapisane pomyœlnie.";
				currentCategory = null; // deselect category and free the form
				await LoadCategoriesList(); // refresh Master-List
				if (AppliesToCheckboxGrid != null) //save the fetched items
					KeyLabelPairsAppliesTo = AppliesToCheckboxGrid.KeyLabelPairs;
			}
			else
			{
				var errorResult = await response.Content.ReadFromJsonAsync<ExceptionCasting>();
				errorMessage = errorResult?.Error?.Message ?? "Wyst¹pi³ b³¹d podczas zapisywania kategorii.";
			}
		}
		catch (Exception ex)
		{
			errorMessage = $"B³¹d komunikacji z API: {ex.Message}";
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