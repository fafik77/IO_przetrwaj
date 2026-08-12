using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Models.Posts;

namespace PrzetrwajPL.Components;

public partial class Home
{
	StatisticsDto? statistics;
	IEnumerable<PostMinimalCategoryRegion>? postsMapData;
	const string PolandMapId = "polandMap";
	const string initializePolandMapJsFunc = "initializePolandMap";
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
			var getStatisticsTask = client.GetAsync("/Statistics");
			var getMapPostsTask = client.GetAsync("/Posts/map");

			var getStatisticsResult = await getStatisticsTask;
			if (getStatisticsResult.IsSuccessStatusCode)
				statistics = await getStatisticsResult.Content.ReadFromJsonAsync<StatisticsDto>();

			var getMapPostsResult = await getMapPostsTask;
			if (getMapPostsResult.IsSuccessStatusCode)
			{
				postsMapData = await getMapPostsResult.Content.ReadFromJsonAsync<IEnumerable<PostMinimalCategoryRegion>>();
			}
		}
		catch (Exception)
		{

		}
	}
}