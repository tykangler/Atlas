using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Atlas.Core.Tests.Utility;

internal static class HtmlUtility
{
    public static async Task<INode> CreateDocument(string html)
    {
        HtmlParser htmlParser = new(new HtmlParserOptions
        {
            IsEmbedded = true,
            IsNotConsumingCharacterReferences = true
        });
        var document = await htmlParser.ParseDocumentAsync(html);
        return document.Body!.FirstElementChild ??
               document.Body!.FirstChild!;
    }
}