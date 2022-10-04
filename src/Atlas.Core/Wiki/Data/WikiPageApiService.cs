using System.Runtime.CompilerServices;
using System.Text.Json;
using Atlas.Core.Exceptions;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Data;

/// <summary>
/// Exposes methods to retrieve wikipedia page data from the api and deserializes responses
/// to defined models
/// </summary>
public class WikiPageApiService
{
    private readonly (string, string)[] defaultQueryParams = new (string, string)[] {
        ( "format", "json" ),
        ( "formatversion", "2" ),
        ( "errorformat", "plaintext" )
    };
    private const string errorsKey = "errors";
    private const string queryKey = "query";
    private const string pagesKey = "pages";
    private const string pageIdKey = "pageId";

    private HttpClient httpClient;

    // use typed client in di config
    public WikiPageApiService(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php/");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Atlas/1.0");
        this.httpClient = httpClient;
    }

    public async IAsyncEnumerable<uint> GetPageIds([EnumeratorCancellation] CancellationToken token = default)
    {
        IEnumerable<(string, string)> queryParams = defaultQueryParams.Concat(new (string, string)[] {
            ("action", "query"),
            ("generator", "allpages"),
            ("gapfilterdir", "nonredirects"),
            ("gaplimit", "max"),
            ("gapminsize", "17000")
        });
        string continueQuery = string.Empty; // query to continue generating pages from
        do
        {
            var response = await this.httpClient.GetWithQueryAsync(queryParams);
            response.EnsureSuccessStatusCode();

            var deserialized = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var root = deserialized.RootElement;
            if (root.TryGetProperty(errorsKey, out var errors)) // get errors
            {
                var wikiErrors = errors.Deserialize<WikiErrorResponse>()!;
                throw new WikiApiException(
                    $"Request {response.RequestMessage?.RequestUri} failed with errors",
                    wikiErrors);
            }
            continueQuery = root
                .GetProperty("continue")
                .GetProperty("gapcontinue").GetString() ?? string.Empty;
            queryParams = queryParams.Append(("gapcontinue", continueQuery));
            var pages = root.GetProperty(queryKey)
                .GetProperty(pagesKey)
                .EnumerateArray();
            foreach (var pageNode in pages)
            {
                yield return pageNode.GetProperty(pageIdKey).GetUInt32();
            }
        } while (!string.IsNullOrEmpty(continueQuery) && !token.IsCancellationRequested);
    }
}