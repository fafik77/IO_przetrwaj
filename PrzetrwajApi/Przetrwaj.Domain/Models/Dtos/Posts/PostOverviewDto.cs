using Przetrwaj.Domain.Entities;

namespace Przetrwaj.Domain.Models.Dtos.Posts;


/// <summary>
/// Contains general data about the post. Usefull for listing a bunch of posts
/// </summary>
public class PostOverviewDto
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public CategoryDto? Category { get; set; }
    public RegionOnlyDto? Region { get; set; }
    public LatLong? LatLong { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public UserGeneralDtoNoRegion? Author { get; set; }


    public long VotePositive { get; set; }
    public long VoteNegative { get; set; }

    public static PostOverviewDto Map(Post p, long VotePositive, long VoteNegative)
    {
        LatLong? latLong = (p.Lat == null) ? null : new LatLong(p.Lat.Value, p.Long!.Value);
        return new PostOverviewDto
        {
            Id = p.IdPost,
            Title = p.Title,
            DateCreated = p.DateCreated,
            LatLong = latLong,
            Author = UserGeneralDtoNoRegion.Map(p.IdAutorNavigation),
            Category = p.CustomCategory.Length > 0 ? new CategoryDto
            {
                Id = p.IdCategory,
                Type = p.IdCategoryNavigation?.Type ?? p.CategoryType,
                Name = p.CustomCategory,
                IsCustom = true,
                Impediments = p.IdCategoryNavigation?.Impediments ?? 0,
            }
            : CategoryDto.Map(p.IdCategoryNavigation),
            Region = RegionOnlyDto.Map(p.RegionNavigation),
            // --- VOTE CALCULATIONS (Executed on Database side) ---
            VotePositive = VotePositive,
            VoteNegative = VoteNegative,
        };
    }
}
