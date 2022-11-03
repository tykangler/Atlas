using System.Text.Json;
using System.Text.Json.Serialization;
using Atlas.Core.Wiki.Data.Models;

namespace Atlas.Core.Wiki.Data.Converters;

public class WikiPageBatchResponseConverter : BaseWikiResponseConverter<WikiPageBatchResponse>
{
    private const string ContinueKey = "continue";
    private const string GapContinueKey = "gapcontinue";
    private const string QueryKey = "query";
    private const string PageIdKey = "pageid";
    private const string TitleKey = "title";
    public override WikiPageBatchResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string continueVal = "";
        bool continueVisited = false;
        IEnumerable<WikiPage> pages = Enumerable.Empty<WikiPage>();
        IEnumerable<WikiError> errors = Enumerable.Empty<WikiError>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                bool propertyIsWarningKey = reader.ValueTextEquals(WarningsKey);
                if (propertyIsWarningKey || reader.ValueTextEquals(ErrorsKey))
                {
                    errors = errors.Concat(this.ReadErrors(ref reader, propertyIsWarningKey));
                }
                else if (reader.ValueTextEquals(ContinueKey) && !continueVisited)
                {
                    continueVal = ParseContinueValue(ref reader);
                    continueVisited = true; // to deal with another 'continue' key inside of 'continue' object
                }
                else if (reader.ValueTextEquals(QueryKey))
                {
                    pages = ParseQueryObject(ref reader);
                }
            }
        }
        return new WikiPageBatchResponse(
            Pages: pages,
            Continue: continueVal,
            Errors: errors
        );
    }

    public override void Write(Utf8JsonWriter writer, WikiPageBatchResponse value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    private string ParseContinueValue(ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(GapContinueKey))
            {
                reader.Read();
                return reader.GetString() ?? "";
            }
        }
        return "";
    }

    private IEnumerable<WikiPage> ParseQueryObject(ref Utf8JsonReader reader)
    {
        List<WikiPage> pages = new();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                pages.Add(ParsePageObject(ref reader));
            }
        }
        return pages;
    }

    private WikiPage ParsePageObject(ref Utf8JsonReader reader)
    {
        int id = -1;
        string title = "";
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                if (reader.ValueTextEquals(PageIdKey))
                {
                    reader.Read();
                    id = reader.GetInt32();
                }
                else if (reader.ValueTextEquals(TitleKey))
                {
                    reader.Read();
                    title = reader.GetString() ?? "";
                }
            }
        }
        return new WikiPage(PageId: id, Title: title);
    }
}
