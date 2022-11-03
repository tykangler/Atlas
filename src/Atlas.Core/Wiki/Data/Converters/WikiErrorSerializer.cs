using System.Text.Json;
using Atlas.Core.Wiki.Data.Models;

namespace Atlas.Core.Wiki.Data.Converters;

public static class WikiErrorSerializer
{
    private const string CodeKey = "code";
    private const string TextKey = "text";
    private const string ModuleKey = "module";

    public static WikiError Read(ref Utf8JsonReader reader, bool isWarning)
    {
        string code = "";
        string text = "";
        string module = "";
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                if (reader.ValueTextEquals(CodeKey))
                {
                    reader.Read();
                    code = reader.GetString() ?? "";
                }
                else if (reader.ValueTextEquals(TextKey))
                {
                    reader.Read();
                    text = reader.GetString() ?? "";
                }
                else if (reader.ValueTextEquals(ModuleKey))
                {
                    reader.Read();
                    module = reader.GetString() ?? "";
                }
            }
        }
        return new WikiError(
            Code: code,
            Text: text,
            Module: module,
            IsWarning: isWarning);
    }
}