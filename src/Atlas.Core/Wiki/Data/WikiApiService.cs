using System.Runtime.CompilerServices;
using System.Text.Json;
using Atlas.Core.Exceptions;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Data;

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
    public async IAsyncEnumerable<uint> GetPageIds([EnumeratorCancellation] CancellationToken cancellationToken = default)
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
                yield return pageNode.GetProperty(pageIdKey).GetUInt32();
            }
        } while (!string.IsNullOrEmpty(continueQuery));
    }

    public async

    private void ThrowIfErrorResponse(JsonElement jsonElement, HttpResponseMessage response)
    {
        if (jsonElement.TryGetProperty(errorsKey, out var errors)) // get errors
        {
            string requestUri = response.RequestMessage?.RequestUri?.ToString() ?? "<unknown>";
            var wikiErrors = errors.Deserialize<IEnumerable<WikiErrorResponse>>()!;
            throw new WikiApiException(
                $"Request {requestUri} failed with errors.",
                wikiErrors);
        }
    }

}