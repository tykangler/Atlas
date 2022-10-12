using System.Text.Json;

namespace Atlas.Core.Extensions;

public static class HttpContentExtensions
{
    public static async Task<JsonDocument> ReadAsJsonDocumentAsync(
        this HttpContent httpContent, CancellationToken cancellationToken)
    {
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
        var deserialized = await JsonDocument.ParseAsync(
            stream, cancellationToken: cancellationToken);
        return deserialized;
    }
}