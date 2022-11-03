using System.Text.Json;
using System.Text.Json.Serialization;
using Atlas.Core.Wiki.Data.Models;

namespace Atlas.Core.Wiki.Data.Converters;

public abstract class BaseWikiResponseConverter<T> : JsonConverter<T>
{
    protected const string ErrorsKey = "errors";
    protected const string WarningsKey = "warnings";

    protected IEnumerable<WikiError> ReadErrors(ref Utf8JsonReader reader, bool isWarning)
    {
        List<WikiError> errors = new();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                errors.Add(WikiErrorSerializer.Read(ref reader, isWarning));
            }
        }
        return errors;
    }
}
