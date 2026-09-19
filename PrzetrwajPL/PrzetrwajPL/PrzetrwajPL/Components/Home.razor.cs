using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Models.Posts;
using System.Collections.Frozen;

namespace PrzetrwajPL.Components;

public partial class Home
{
    StatisticsDto? statistics;
    IEnumerable<PostMinimalNamedCategoryRegion>? postsMapData;
    const string PolandMapId = "polandMap";
    const string initializePolandMapJsFunc = "initializePolandMap";
    const string CategoriesEndpoint = "/Categories";
    const string StatisticsEndpoint = "/Statistics";
    const string PostsMapEndpoint = "/Posts/map";
    [Inject] public required IJSRuntime JS { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await FetchData();
            StateHasChanged();
            if (postsMapData != null)
            {
                await JS.InvokeVoidAsync(initializePolandMapJsFunc, PolandMapId, postsMapData);
            }
        }
    }

    private async Task FetchData()
    {
        try
        {
            var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
            var getStatisticsTask = client.GetAsync(StatisticsEndpoint);
            var getMapPostsTask = client.GetAsync(PostsMapEndpoint);
            var getCategoriesTask = client.GetAsync(CategoriesEndpoint);

            await Task.WhenAll(getStatisticsTask, getMapPostsTask, getCategoriesTask);

            var getStatisticsResult = await getStatisticsTask;
            if (getStatisticsResult.IsSuccessStatusCode)
                statistics = await getStatisticsResult.Content.ReadFromJsonAsync<StatisticsDto>();

            FrozenDictionary<int, CategoryDto> categoriesDict = FrozenDictionary<int, CategoryDto>.Empty;
            var getCategoriesResult = await getCategoriesTask;
            if (getCategoriesResult.IsSuccessStatusCode)
            {
                var categories = await getCategoriesResult.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
                categoriesDict = categories!.ToFrozenDictionary(c => c.Id);
            }

            var getMapPostsResult = await getMapPostsTask;
            if (getMapPostsResult.IsSuccessStatusCode)
            {
                postsMapData = await getMapPostsResult.Content.ReadFromJsonAsync<IEnumerable<PostMinimalNamedCategoryRegion>>();
                foreach (var item in postsMapData!)
                {
                    CategoryDto? categoryDto = categoriesDict.GetValueOrDefault(item.IdCategory);
                    item.CategoryName = categoryDto?.Name;
                    item.IsResource = categoryDto?.Type == CategoryType.Resource;
                }
            }
        }
        catch (Exception)
        {

        }
    }
}