using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models.Dtos.Posts;

namespace Przetrwaj.Tests.Unit.Mapping;

public class PostOverview
{
    [Fact]
    public void CustomCategory_Preserves_Impediments_In_PostOverviewDto_Map()
    {
        var category = new CategoryDanger
        {
            IdCategory = 1,
            Name = "Inne",
            Impediments = 0b0101 // impediments 0 and 2
        };

        var post = new Post
        {
            IdPost = "post-custom",
            Title = "Custom Category Post",
            Description = "Test description",
            CategoryType = CategoryType.Danger,
            IdCategory = category.IdCategory,
            IdCategoryNavigation = category,
            CustomCategory = "Moja kategoria",
            IdAutor = "user-1",
            IdAutorNavigation = new AppUser { Id = "user-1", UserName = "tester" },
            Active = true
        };

        var dto = PostOverviewDto.Map(post, 0, 0);

        Assert.NotNull(dto.Category);
        Assert.Equal(post.CustomCategory, dto.Category.Name);
        Assert.True(dto.Category.IsCustom);
        Assert.Equal(category.Impediments, dto.Category.Impediments);
    }
}
