using System.Diagnostics;
using Atlas.Core.Exceptions;
using Atlas.Core.Wiki;
using Atlas.Core.Wiki.Models;

namespace Atlas.Core.Tests.Wiki.Data;

public class WikiApiServiceTests
{
    [Trait("Category", "ApiService")]
    [Fact]
    public async Task WhenGetPages_PagesAreReturned()
    {
        var apiService = new WikiApiService(new HttpClient());
        var result = await apiService.GetPages();
        Assert.NotNull(result.Continue);
        Assert.NotNull(result.Query?.Pages);
        Assert.Null(result.Errors);
        Assert.Null(result.Warnings);

        Assert.NotEmpty(result.Query!.Pages);
    }

    [Trait("Category", "ApiService")]
    [Fact]
    public async Task WhenGetPages_ContinueIsPopulated()
    {
        var apiService = new WikiApiService(new HttpClient());
        var result = await apiService.GetPages();
        Assert.NotNull(result.Continue);
        Assert.False(string.IsNullOrWhiteSpace(result.Continue.GapContinue));
    }

    [Trait("Category", "ApiService")]
    [InlineData(10000)]
    [Theory]
    public async Task WhenMakeMultipleRequests_NotRateLimited(long milliSecondsTimeout)
    {
        var apiService = new WikiApiService(new HttpClient());
        List<WikiGetPageResponse> pages = new();
        var stopWatch = Stopwatch.StartNew();
        while (stopWatch.ElapsedMilliseconds < milliSecondsTimeout)
        {
            pages.Add(await apiService.GetPages());
        }
        Assert.DoesNotContain(null, pages);
    }
}