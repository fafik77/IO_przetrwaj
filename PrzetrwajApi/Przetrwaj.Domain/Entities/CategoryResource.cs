namespace Przetrwaj.Domain.Entities;

public partial class CategoryResource : Category
{
	public CategoryResource() : base()
	{
		Type = CategoryType.Resource;
	}
}
