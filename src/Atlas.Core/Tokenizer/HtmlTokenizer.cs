namespace Atlas.Core.Tokenizer;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Atlas.Core.Tokenizer.Input;
using Atlas.Core.Tokenizer.Token;
using Atlas.Core.Tokenizer.Validators;
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

    /* 
    this method should never change.
    no creation logic here.
    1. If html element invalid, then continue.
    2. Create Node.
        2a. There may be cases where each node will have their own recursive create steps because they have children.
        2b. This 'create' step will likely have all the same operations as the overall extract method.
        2c. How to handle this?
        2d. Maybe just recreate the logic. And break apart the main processing logic into separate pieces.
        2e. foreach child node, validate. create node. if not enough information, recurse down again. 
    3. If node null (not enough information) => then recurse down. 
    4. Merge Text Nodes. If merging from step 3, then this will be a list to merge. If merging from step 2, then this will be a single element.
    5. Add to list.
    The valiator can be its own component, the creation can be in either the tokens or a factory method. 
    */

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
            return await ElementTokenizer.TokenizeElement(starterNode);
        }
        else
        {
            return await ElementTokenizer.TokenizeElement(htmlDocument.Node);
        }
    }


    private static INode? GetStarterNode(IHtmlDocument document) => document.QuerySelector(StarterNodeClass);

    private static TextNode MergeTextNodes(TextNode node1, TextNode node2) => new($"{node1.Value} {node2.Value}");

}