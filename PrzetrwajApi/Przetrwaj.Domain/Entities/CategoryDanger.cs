namespace Przetrwaj.Domain.Entities;

public partial class CategoryDanger : Category
{
	public CategoryDanger() : base()
	{
		Type = CategoryType.Danger;
	}
}
