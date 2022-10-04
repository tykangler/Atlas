using Microsoft.AspNetCore.WebUtilities;

namespace Atlas.Core.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> GetWithQueryAsync(this HttpClient client, IEnumerable<(string, string)> queryParams)
    {
        var queryString = string.Empty;
        foreach (var (key, value) in queryParams)
        {
            QueryHelpers.AddQueryString(queryString, key, value);
        }
        return await client.GetAsync(queryString);
    }
}