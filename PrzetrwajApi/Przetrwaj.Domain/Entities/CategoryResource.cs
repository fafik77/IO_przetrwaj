namespace Przetrwaj.Domain.Entities;

public class CategoryResource : Category
{
	public CategoryResource() : base()
	{
		Type = CategoryType.Resource;
	}
}
