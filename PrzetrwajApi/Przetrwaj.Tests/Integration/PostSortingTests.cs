using Microsoft.EntityFrameworkCore;
using Przetrwaj.Domain.Abstractions;
using Przetrwaj.Domain.Entities;
using Przetrwaj.Domain.Models;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Infrastucture.Context;
using Przetrwaj.Infrastucture.Repositories;

namespace Przetrwaj.Tests.Integration;

public class PostSortingTests : IDisposable
{
    ApplicationDbContext context;
    DummyRegionRepository regionRepo;
    PostRepository postRepo;

    public PostSortingTests()
    {
        context = CreateDbContext();
        regionRepo = new DummyRegionRepository();
        postRepo = new PostRepository(context, regionRepo);
    }
    public void Dispose()
    {
        context.Dispose();
    }

    private class DummyRegionRepository : IRegionRepository
    {
        public Task<AllRegions> GetAllAsync(CancellationToken ct) => throw new NotImplementedException();
        public Task<IRegionInfo?> GetByIdAsync(int id, CancellationToken ct) => Task.FromResult<IRegionInfo?>(null);
        public Task<IRegionInfo?> RegionFromLocationAsync(LatLong location, CancellationToken ct = default) => Task.FromResult<IRegionInfo?>(null);
        public Task AddAsync<T>(IEnumerable<T> regions, CancellationToken ct) where T : class, IRegionInfo => Task.CompletedTask;
        public Task AddAsync<T>(T region, CancellationToken ct) where T : class, IRegionInfo => Task.CompletedTask;
        public void Delete<T>(IEnumerable<T> regions) where T : class, IRegionInfo { }
        public void Delete<T>(T region) where T : class, IRegionInfo { }
        public void Update<T>(IEnumerable<T> regions) where T : class, IRegionInfo { }
        public void Update<T>(T region) where T : class, IRegionInfo { }
    }

    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetMatchingPostsAsync_SortsBy_DateOnlyDesc_Then_ImpedimentsMatchDesc()
    {
        // setup
        await CreateTestData();

        // Filter impediment mask = 0b0011 (bits 0 and 1)
        int filterImpediments = 0b0011;

        // when
        var filter = new MatchingPostsFilter
        {
            RegionId = 0,
            Impediment = filterImpediments,
            CategoryFilter = CategoryTypeFilter.Danger
        };

        var results = (await postRepo.GetMatchingPostsAsync(filter)).ToList();

        // then
        // Expected order:
        // 1. (Today, 2 matches)
        // 2. (Today, 1 match) - matches the user's explicit example:
        //    "Example there are 2 posts with the same date, one has category matching only 1 "filter.impediment"
        //     -that post will be after a post that has a category matching 2 and more of "filter.impediment"."
        // 3. (Today, 0 matches)
        // 4. (Yesterday, 2 matches - older date)
        Assert.Equal("2", results[0].Id);
        Assert.Equal("1", results[1].Id);
        Assert.Equal("0", results[2].Id);
    }

    [Fact]
    public async Task SameDate_And_SameImpedimentMatch_SortsBy_DateCreatedDesc()
    {
        // setup
        await CreateTestData();

        // when
        var filter = new MatchingPostsFilter
        {
            RegionId = 0,
            Impediment = 0b0001,
            CategoryFilter = CategoryTypeFilter.Danger
        };

        var results = (await postRepo.GetMatchingPostsAsync(filter)).ToList();

        // then
        Assert.Equal("2", results[0].Id);
        Assert.Equal("1", results[1].Id);
    }

    [Fact]
    public async Task FilterImpediment_NullOrZero_SortsBy_DateCreatedDesc()
    {
        // setup
        await CreateTestData();

        // when
        var filterNull = new MatchingPostsFilter
        {
            RegionId = 0,
            Impediment = null,
            CategoryFilter = CategoryTypeFilter.Danger
        };

        var results = (await postRepo.GetMatchingPostsAsync(filterNull)).ToList();

        // then
        Assert.Equal("2", results[0].Id);
        Assert.Equal("1", results[1].Id);
    }

    private async Task CreateTestData()
    {

        var author = new AppUser { Id = "author-3", UserName = "author3" };
        context.Users.Add(author);

        var cat0 = new CategoryDanger { IdCategory = 1, Name = "Cat0", Impediments = 0b0000 };
        var catA = new CategoryDanger { IdCategory = 30, Name = "CatA", Impediments = 0b0001 };
        var catB = new CategoryDanger { IdCategory = 31, Name = "CatB", Impediments = 0b0111 };
        context.Categories.AddRange(cat0, catA, catB);

        var region0 = new RegionWoj { Id = 0, Name = "0" };
        context.Regions.Add(region0);

        var today = new DateTimeOffset(2026, 9, 13, 0, 0, 0, TimeSpan.Zero);

        //yesterday
        var post0 = new Post
        {
            IdPost = "0",
            Title = "Post 0",
            Description = "desc",
            CategoryType = CategoryType.Danger,
            IdCategory = cat0.IdCategory,
            IdCategoryNavigation = cat0,
            IdAutor = author.Id,
            DateCreated = today.AddDays(-1).AddHours(10),
            Active = true,
            IdRegion = 0
        };

        //today at 10:00
        var post1 = new Post
        {
            IdPost = "1",
            Title = "Post 1",
            Description = "desc",
            CategoryType = CategoryType.Danger,
            IdCategory = catA.IdCategory,
            IdCategoryNavigation = catA,
            IdAutor = author.Id,
            DateCreated = today.AddHours(10),
            Active = true,
            IdRegion = 0
        };

        //today at 12:00
        var post2 = new Post
        {
            IdPost = "2",
            Title = "Post 2",
            Description = "desc",
            CategoryType = CategoryType.Danger,
            IdCategory = catB.IdCategory,
            IdCategoryNavigation = catB,
            IdAutor = author.Id,
            DateCreated = today.AddHours(12),
            Active = true,
            IdRegion = 0
        };

        context.Posts.AddRange(post0, post1, post2);
        await context.SaveChangesAsync();
    }

}
