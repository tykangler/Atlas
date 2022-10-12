using Microsoft.AspNetCore.WebUtilities;

namespace Atlas.Core.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> GetWithQueryAsync(
        this HttpClient client, IEnumerable<(string, string)> queryParams, CancellationToken cancellationToken = default)
    {
        var queryString = string.Empty;
        foreach (var (key, value) in queryParams)
        {
            queryString = QueryHelpers.AddQueryString(queryString, key, value);
        }
        if (cancellationToken != default)
        {
            return await client.GetAsync(queryString, cancellationToken);
        }
        return await client.GetAsync(queryString);
    }
}