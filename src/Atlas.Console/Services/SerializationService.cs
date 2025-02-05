namespace Atlas.Console.Services;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class SerializationService
{
    public static string Serialize<T>(T obj, bool indented = true)
    => JsonSerializer.Serialize(obj,
        new JsonSerializerOptions
        {
            WriteIndented = indented,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        });

    public static T? Deserialize<T>(Stream stream)
    => JsonSerializer.Deserialize<T>(stream,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
}