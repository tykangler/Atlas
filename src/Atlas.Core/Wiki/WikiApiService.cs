using System.Net.Http.Json;
using System.Text.Json;
using Atlas.Core.Exceptions;
using Atlas.Core.Extensions;
using Atlas.Core.Wiki.Models;

namespace Atlas.Core.Wiki;

/// <summary>
/// Exposes methods to retrieve wikipedia page data from the api and deserializes responses
/// to defined models
/// </summary>
public class WikiApiService
{
    private readonly (string, string)[] defaultQueryParams = new (string, string)[] {
        ( "format", "json" ),
        ( "formatversion", "2" ),
        ( "errorformat", "plaintext" )
    };
    private readonly JsonSerializerOptions deserializeOptions = new(JsonSerializerDefaults.Web);
    private const string errorsKey = "errors";
    private const string queryKey = "query";
    private const string pagesKey = "pages";
    private const string pageIdKey = "pageid";
    private const int defaultMinBytesPerPage = 17000;
    private const int defaultMaxPages = 500;

    private HttpClient httpClient;

    // use typed client in di config
    public WikiApiService(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php/");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Atlas/1.0");
        this.httpClient = httpClient;
    }

    /// <summary>
    /// Gets all page ids of sizable content
    /// </summary>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Page ids</returns>
    /// <exception cref="OperationCanceledException">If task is cancelled</exception>
    /// <exception cref="HttpRequestException">
    ///     If request fails due to underlying network issue, or if status code is not 200
    /// </exception>
    /// <exception cref="InvalidOperationException">If request already set</exception>
    /// <exception cref="WikiApiException">If response is an error response as defined by mediawiki</exception>
    public async Task<WikiGetPageResponse> GetPages(
        string continueFrom = "",
        int minBytesPerPage = defaultMinBytesPerPage,
        int maxPages = defaultMaxPages,
        CancellationToken cancellationToken = default)
    {
        var queryParams = defaultQueryParams.Concat(new (string, string)[] {
            ("action", "query"),
            ("generator", "allpages"),
            ("gapfilterredir", "nonredirects"),
            ("gapminsize", minBytesPerPage.ToString()),
            ("gaplimit", maxPages.ToString())
        });
        if (!string.IsNullOrWhiteSpace(continueFrom))
        {
            queryParams = queryParams.Append(("gapcontinue", continueFrom));
        }
        return await GetWikiResponse<WikiGetPageResponse>(queryParams, cancellationToken);
    }

    public async Task<WikiParseResponse> ParsePageFromIdAsync(string pageId, CancellationToken cancellationToken = default)
    {
        var queryParams = defaultQueryParams.Concat(new (string, string)[] {
                ("action", "parse"),
                ("prop", "text|categories"),
                ("pageid", pageId),
                ("redirects", "true"),
                // disabled sections
                ("disableeditsection", "true"),
                ("disabletoc", "true"),
                ("disablelimitreport", "true"),
            });
        return await GetWikiResponse<WikiParseResponse>(queryParams, cancellationToken);
    }

    public async Task<WikiParseResponse> ParsePageFromTitleAsync(string pageTitle, CancellationToken cancellationToken = default)
    {
        var queryParams = defaultQueryParams.Concat(new (string, string)[] {
                ("action", "parse"),
                ("prop", "text|categories"),
                ("page", pageTitle),
                ("redirects", "true"),
                // disabled sections
                ("disableeditsection", "true"),
                ("disabletoc", "true"),
                ("disablelimitreport", "true"),
            });
        return await GetWikiResponse<WikiParseResponse>(queryParams, cancellationToken);
    }

    private async Task<T> GetWikiResponse<T>(
        IEnumerable<(string, string)> queryParams,
        CancellationToken cancellationToken = default)
    where T : BaseWikiResponse
    {
        var response = await this.httpClient.GetWithQueryAsync(queryParams, cancellationToken);
        response.EnsureSuccessStatusCode();
        var wikiResponse = await response.Content.ReadFromJsonAsync<T>(deserializeOptions);
        string requestUri = response.RequestMessage?.RequestUri?.ToString() ?? "<unknown>";

        if (wikiResponse == null || AnyErrorsPresent(wikiResponse))
        {
            throw new WikiApiException(
                $"Request {requestUri} failed with errors",
                wikiResponse?.Errors
            );
        }
        return wikiResponse;
    }

    private bool AnyErrorsPresent(BaseWikiResponse wikiResponse) =>
        wikiResponse.Errors != null && wikiResponse.Errors.Any();
}
