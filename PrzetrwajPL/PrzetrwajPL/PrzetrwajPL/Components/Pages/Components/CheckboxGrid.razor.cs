namespace PrzetrwajPL.Components.Pages.Components;

public partial class CheckboxGrid
{
	private Dictionary<int, CheckboxItem> _slots = new();
	public Dictionary<int, string> KeyLabelPairs { get; protected set; } = new();

	/// <summary>
	/// Dynamicly updates checkbox in slot [0; 31]
	/// </summary>
	public void AddCheckbox(int number, string label, bool updateState = true)
	{
		if (number < 0 || number > 31)
			throw new ArgumentOutOfRangeException(nameof(number), "Number has to be in [0; 31] range!");
		_slots[number] = new CheckboxItem
		{
			Number = number,
			Label = label,
			IsChecked = false
		};
		if (updateState)
			StateHasChanged();
	}

	/// <summary>
	/// Returns a list of (IDs) of checked boxes
	/// </summary>
	public List<int> GetSelectedNumbers()
	{
		return _slots.Values
			.Where(x => x.IsChecked)
			.Select(x => x.Number)
			.ToList();
	}

	/// <summary>
	/// Clears all labels
	/// </summary>
	public void ClearGrid()
	{
		_slots.Clear();
		StateHasChanged();
	}

	private class CheckboxItem
	{
		public int Number { get; set; }
		public string Label { get; set; } = string.Empty;
		public bool IsChecked { get; set; }
	}

	public void CheckItem(int index, bool check) => _slots[index].IsChecked = check;

	public void LoadKeyLabelPairs(Dictionary<int, string> KeyLabelPairs)
	{
		if (KeyLabelPairs.Count == 0) return;
		_slots.Clear();
		this.KeyLabelPairs = KeyLabelPairs;
		foreach (var item in KeyLabelPairs)
			AddCheckbox(item.Key, item.Value, updateState: false);
		StateHasChanged();
	}

	protected void LoadFromIntBitField(int bits)
	{
		int pos = 0;
		while (bits != 0)
		{
			if ((bits & 1) != 0)
			{
				CheckItem(pos, true);
			}
			bits >>= 1;
			pos++;
		}
		StateHasChanged();
	}

	public void LoadFromIntBitField(int bits, Dictionary<int, string> KeyLabelPairs)
	{
		LoadKeyLabelPairs(KeyLabelPairs);
		LoadFromIntBitField(bits);
	}

	public int GetSelectedAsIntBitField()
	{
		int retVal = 0;
		foreach (var item in _slots)
		{
			if (item.Value.IsChecked)
				retVal |= (1 << item.Key);
		}
		return retVal;
	}
}