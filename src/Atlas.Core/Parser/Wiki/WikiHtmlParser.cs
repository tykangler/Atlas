using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Atlas.Core.Extensions;
using Atlas.Core.Model.Annotations;
using Atlas.Core.Parser.Wiki.HtmlHandlers;
using Document = Atlas.Core.Model.Document;

namespace Atlas.Core.Parser.Wiki;

public class WikiHtmlParser : IParser
{
    private static readonly string[] IgnoreClasses = { "noprint", "hatnote", "reference", "printfooter", "mw-references-wrap", "reflist" };
    private static readonly string[] IgnoreTags = { "style", "cite", "sup", "link", "script", "noscript" };

    private readonly HtmlParser htmlParser;
    private readonly IEnumerable<IWikiHtmlHandler> htmlHandlers;

    public WikiHtmlParser(IEnumerable<IWikiHtmlHandler>? htmlHandlers = null)
    {
        htmlParser = new HtmlParser(new HtmlParserOptions
        {
            IsKeepingSourceReferences = false,
            IsEmbedded = true,
            IsScripting = false,
        });
        this.htmlHandlers = htmlHandlers ?? Enumerable.Empty<IWikiHtmlHandler>();
    }

    public async Task<Document> Parse(string inputDoc)
    {
        var parsedHtml = await htmlParser.ParseDocumentAsync(inputDoc);
        var rootElement = parsedHtml.Body?.Children?.FirstOrDefault();
        if (rootElement == null)
        {
            throw new InvalidOperationException("Could not parse input document");
        }
        var parseResult = ParseNode(rootElement);
        return new Document(
            Raw: inputDoc,
            Parsed: parseResult?.Parsed ?? string.Empty,
            Annotations: parseResult?.Annotations ?? AnnotationCollection.Empty);
    }

    private WikiHtmlParseResult? ParseNode(INode rootNode)
    {
        if (rootNode is IElement element && ShouldIgnore(element))
        {
            return null;
        }
        else
        {
            var document = HandleNode(rootNode);
            if (document != null)
            {
                return document;
            }
        }
        var parsedChildren = rootNode.ChildNodes.Select(ParseNode).Where(parse => parse != null);
        return MergeParseResults(parsedChildren!);
    }

    private WikiHtmlParseResult? HandleNode(INode node)
    {
        foreach (var htmlHandler in htmlHandlers)
        {
            var document = htmlHandler.Handle(node);
            if (document != null)
            {
                return document;
            }
        }
        return null;
    }

    private bool ShouldIgnore(IElement element)
    => IgnoreTags.Any(element.IsTag)
    || IgnoreClasses.Any(targetClass => element.HasClass(targetClass));

    private WikiHtmlParseResult MergeParseResults(IEnumerable<WikiHtmlParseResult> parseResults)
        => parseResults.Aggregate(WikiHtmlParseResult.Empty, (final, next) => final with
        {
            Parsed = final.Parsed + next.Parsed,
            Annotations = MergeAnnotations(final.Annotations, next.Annotations, final.Parsed.Length)
        });

    private AnnotationCollection MergeAnnotations(AnnotationCollection first, AnnotationCollection second, int offsetLength)
    {
        var newSecondAnnotations = second.Select(annotation => annotation with
        {
            StartIndex = annotation.StartIndex + offsetLength,
            EndIndex = annotation.EndIndex + offsetLength
        });
        return new AnnotationCollection(first.Concat(newSecondAnnotations));
    }
}