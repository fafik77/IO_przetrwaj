using Microsoft.AspNetCore.Components;
using PrzetrwajPL.Models;

namespace PrzetrwajPL.Components.Pages.Components
{
    public partial class TypePicker
    {
        [Parameter] public int SelectedTypeId { get; set; } = 0;
        [Parameter] public EventCallback<int> SelectedTypeIdChanged { get; set; }

        // Display Name
        public string SelectedTypeName { get; private set; } = "Zagrożenia";

        private bool isDropdownOpen = false;
        private List<String> types = ["Zagrożenia", "Zasoby", "Zagrożenia i Zasoby"];

        private void ToggleDropdown() => isDropdownOpen = !isDropdownOpen;
        private async Task SelectType(int id, string name) => await PickType(id);

        public async Task PickType(int id)
        {
            SelectedTypeName = types.ElementAt(id);
            SelectedTypeId = id;
            isDropdownOpen = false;
            await SelectedTypeIdChanged.InvokeAsync(id);
            StateHasChanged(); // Force UI update
        }
    }
}
