namespace Atlas.Core.Tokenizer;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Atlas.Core.Tokenizer.Input;
using Atlas.Core.Tokenizer.Token;
using Document = Input.Document;

/// <summary>
/// Parses through an html page from wikipedia and builds an ordered collection of WikiToken's
/// with respect to their order in the wikipedia document.
/// WikiTokenType 
/// </summary>
public class HtmlTokenizer : ITokenizer
{
    private const string StarterNodeClass = ".mw-parser-output";
    private readonly HtmlParser htmlParser;

    public HtmlTokenizer()
    {
        htmlParser = new HtmlParser(new HtmlParserOptions
        {
            IsEmbedded = true
        });
    }

    public async Task<IEnumerable<WikiToken>> Tokenize(Document document)
    {
        if (document is not HtmlDocument htmlDocument)
        {
            throw new ArgumentException("Document must be HtmlDocument", nameof(document));
        }
        else if (htmlDocument.Node == null)
        {
            var parsedHtmlDocument = await htmlParser.ParseDocumentAsync(document.Content);
            var starterNode = GetStarterNode(parsedHtmlDocument);
            if (starterNode == null)
            {
                throw new ArgumentException("The string html document must contain an element with class '.mw-parser-output'", nameof(document));
            }
            return ElementTokenizer.Tokenize(starterNode);
        }
        else
        {
            return ElementTokenizer.Tokenize(htmlDocument.Node);
        }
    }


    private static INode? GetStarterNode(IHtmlDocument document) => document.QuerySelector(StarterNodeClass);
}