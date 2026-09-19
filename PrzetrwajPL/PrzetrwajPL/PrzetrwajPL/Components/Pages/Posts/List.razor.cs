using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models.Posts;
using PrzetrwajPL.Components.Pages.Components;
using System.Collections.Generic;
using System.Linq;

namespace PrzetrwajPL.Components.Pages.Posts;

public partial class List
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    private void NavigateToPost(string postId)
    {
        Nav.NavigateTo($"/posts/{postId}");
    }

    // Access basic user info from the cookie claims
    [CascadingParameter]
    private Task<AuthenticationState> AuthStateTask { get; set; }
    private const double DebounceDalaySec = 1;


    private RegionPicker? regionPicker;
    private DangerResourceTypePicker? dangerResourceTypePicker;
    private VisibilityPicker? visibilityPicker;

    private IEnumerable<PostOverviewDto>? posts;
    private CancellationTokenSource? debounceCts;
    private bool startedLoadingPosts = false;
    private string? errorMsg = null;
    private HashSet<string> archivingPostIds = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await GetUserInfo();
        }
    }

    // Get logged-in user region and apply it
    private async Task GetUserInfo()
    {
        try
        {
            var authState = await AuthStateTask;
            var userPrincipal = authState.User;

            if (userPrincipal.Identity is not { IsAuthenticated: true })
                return;

            var regionStr = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimNames.Region)?.Value;
            if (int.TryParse(regionStr, out int region))
            {
                await regionPicker!.LoadRegionFromId(region);
            }
        }
        catch (Exception) { }
    }
    /// <summary>
    /// Waits n sec before applying changes
    /// </summary>
    /// <returns>a task</returns>
    private async Task SelectedRegionScopeChanged()
    {
        //debounce for n seconds
        debounceCts?.Cancel();
        debounceCts?.Dispose();
        debounceCts = new CancellationTokenSource();
        var token = debounceCts.Token;
        try
        {
            if (regionPicker?.SelectedRegionId > 0)
                startedLoadingPosts = true;
            //start the cooldonw
            await Task.Delay((int)(DebounceDalaySec * 1_000), token);
            //after (resetable) n seconds
            await LoadPosts();
        }
        catch (TaskCanceledException)
        {
            //when it was refreshed it throws this exception
        }
    }

    private async Task LoadPosts()
    {
        if (visibilityPicker is null) return;
        errorMsg = null;
        posts = null;
        var regionScopeMin = visibilityPicker.Min;
        var regionScopeMax = visibilityPicker.Max;
        var regionId = regionPicker!.GetRegionId(regionScopeMin);
        var dangerResourceType = dangerResourceTypePicker!.categoryType;
        if (regionId == null || regionId < 0)
        {
            startedLoadingPosts = false; return;
        }
        startedLoadingPosts = true;

        var authState = await AuthStateTask;
        var userPrincipal = authState.User;
        var impedimentStr = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimNames.Impediments)?.Value;
        int? impediments = null;
        if (int.TryParse(impedimentStr, out var result)) impediments = result;

        var queryParams = new GetMatchingPostsRequest
        {
            Category = dangerResourceType,
            Impediment = impediments,
            MaxLevel = regionScopeMax,
            RegionId = regionId.Value,
        };

        try
        {
            var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
            var res = await client.GetAsync(
                QueryHelpers.AddQueryString("/Posts", queryParams.ToQueryDictionary())
            );
            if (!res.IsSuccessStatusCode)
            {
                errorMsg = "Wystąpił problem podczas ładowania postów.";
                return;
            }
            posts = await res.Content.ReadFromJsonAsync<IEnumerable<PostOverviewDto>>();
        }
        catch (Exception)
        {

        }
    }

    private async Task ArchivePost(string postId, string postTitle)
    {
        if (archivingPostIds.Contains(postId)) return;

        bool confirmed = await JS.InvokeAsync<bool>("confirm", $"Czy na pewno chcesz zarchiwizować post \"{postTitle}\"?");
        if (!confirmed) return;

        archivingPostIds.Add(postId);
        try
        {
            var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
            var response = await client.PutAsync($"/Posts/{postId}/mark-inactive", null);

            if (response.IsSuccessStatusCode)
            {
                if (posts != null)
                {
                    posts = posts.Where(p => p.Id != postId).ToList();
                    StateHasChanged();
                }
            }
            else
            {
                errorMsg = $"Błąd podczas archiwizacji posta {postId}: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            errorMsg = $"Błąd archiwizacji posta: {ex.Message}";
        }
        finally
        {
            archivingPostIds.Remove(postId);
        }
    }
}