using System.Runtime.CompilerServices;
using System.Text.Json;
using Atlas.Core.Exceptions;
using Atlas.Core.Extensions;
using Atlas.Core.Wiki.Data.Models;

namespace Atlas.Core.Wiki.Data;

/// <summary>
/// Exposes methods to retrieve wikipedia page data from the api and deserializes responses
/// to defined models
/// </summary>
// TODO: Add handing for warnings
public class WikiApiService
{
    private readonly (string, string)[] defaultQueryParams = new (string, string)[] {
        ( "format", "json" ),
        ( "formatversion", "2" ),
        ( "errorformat", "plaintext" )
    };
    private const string errorsKey = "errors";
    private const string queryKey = "query";
    private const string pagesKey = "pages";
    private const string pageIdKey = "pageid";

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
    public async IAsyncEnumerable<WikiPageResponse> GetPageIdsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // TODO: change this to be done in batches so that we guarantee serial request timing, not parallel
        var queryParams = defaultQueryParams.Concat(new (string, string)[] {
            ("action", "query"),
            ("generator", "allpages"),
            ("gapfilterdir", "nonredirects"),
            ("gaplimit", "max"),
            ("gapminsize", "17000")
        });
        string continueQuery = string.Empty; // query to continue generating pages from
        var deserializeOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await this.httpClient.GetWithQueryAsync(queryParams, cancellationToken);
            response.EnsureSuccessStatusCode();
            var root = (await response.Content.ReadAsJsonDocumentAsync(cancellationToken)).RootElement;
            ThrowIfErrorResponse(root, response);
            continueQuery = root
                .GetProperty("continue")
                .GetProperty("gapcontinue").GetString() ??
                string.Empty;
            queryParams = queryParams.Append(("gapcontinue", continueQuery));
            var pages = root.GetProperty(queryKey)
                .GetProperty(pagesKey)
                .EnumerateArray();
            foreach (var pageNode in pages)
            {
                yield return pageNode.Deserialize<WikiPageResponse>(deserializeOptions)!;
            }
        } while (!string.IsNullOrEmpty(continueQuery));
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
        return await ParsePageWithQueryParamsAsync(queryParams, cancellationToken);
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
        return await ParsePageWithQueryParamsAsync(queryParams, cancellationToken);
    }

    private async Task<WikiParseResponse> ParsePageWithQueryParamsAsync(
        IEnumerable<(string, string)> queryParams, CancellationToken cancellationToken = default)
    {
        var response = await this.httpClient.GetWithQueryAsync(queryParams, cancellationToken);
        response.EnsureSuccessStatusCode();
        var root = (await response.Content.ReadAsJsonDocumentAsync(cancellationToken)).RootElement;
        ThrowIfErrorResponse(root, response);

        JsonSerializerOptions deserializeOptions = new(JsonSerializerDefaults.Web);
        var parseResponse = root.GetProperty("parse").Deserialize<WikiParseResponse>(deserializeOptions)!;
        return parseResponse;
    }

    private void ThrowIfErrorResponse(JsonElement jsonElement, HttpResponseMessage response)
    {
        if (jsonElement.TryGetProperty(errorsKey, out var errors)) // get errors
        {
            JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
            string requestUri = response.RequestMessage?.RequestUri?.ToString() ?? "<unknown>";
            var wikiErrors = errors.Deserialize<IEnumerable<WikiErrorResponse>>(options)!;
            throw new WikiApiException(
                $"Request {requestUri} failed with errors.",
                wikiErrors);
        }
    }
}