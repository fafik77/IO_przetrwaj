namespace PrzetrwajPL.Models;

public class CategoryDto
{
	public int IdCategory { get; set; }
	public string Name { get; set; } = null!;
	public CategoryType Type { get; set; }
}