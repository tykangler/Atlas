using Atlas.Core.Wiki.Data;

namespace Atlas.Core.Tests.Wiki.Data;

public class WikiApiServiceTests
{
    [Trait("Category", "ApiService")]
    [Fact]
    public void WhenTimeoutQueryOnPageIds_PageIdsAreReturned()
    {
        var apiService = new WikiApiService(new HttpClient());
    }
}