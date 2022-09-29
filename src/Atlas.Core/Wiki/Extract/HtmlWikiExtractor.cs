namespace Atlas.Core.Wiki.Extract.AST;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;

/// <summary>
/// Parses through an html page from wikipedia and builds an ordered collection of WikiNode's
/// with respect to their order in the wikipedia document.
/// WikiTokenType 
/// </summary>
public class HtmlWikiExtractor : IWikiExtractor
{
    private readonly string[] disallowClasses = {
        "infobox", "reflist", "reference", "reference-text", "hatnote"
    };
    private readonly string[] disallowTags = { "style", "sup", "img" };

    private HtmlParser htmlParser;

    public HtmlWikiExtractor()
    {
        htmlParser = new HtmlParser(new HtmlParserOptions
        {
            IsEmbedded = true,
            IsNotConsumingCharacterReferences = true
        });
    }

    public async Task<IEnumerable<WikiNode>> Extract(string htmlDoc)
    {
        var document = await this.htmlParser.ParseDocumentAsync(htmlDoc);
        if (document.Body?.FirstElementChild != null)
        {
            // build node list
            return Extract(document.Body.FirstElementChild); // div.mw-parser-output
        }
        return Enumerable.Empty<WikiNode>();
    }

    private IEnumerable<WikiNode> Extract(IElement htmlElement)
    {
        List<WikiNode> wikiNodes = new();
        foreach (var childHtmlNode in htmlElement.ChildNodes)
        {
            if (TextNode.TryParse(childHtmlNode, out var textNode))
            {
                wikiNodes.Add(textNode!);
            }
            else if (childHtmlNode is IElement elem)
            {
                if (!disallowTags.Contains(elem.TagName) &&
                    !disallowClasses.Any(elem.ClassList.Contains))
                {
                    if (SectionNode.TryParse(elem, out var sectionNode))
                    {
                        wikiNodes.Add(sectionNode!);
                    }
                    else if (InterLinkNode.TryParse(elem, out var interlinkNode))
                    {
                        wikiNodes.Add(interlinkNode!);
                    }
                    else if (ListNode.TryParse(elem, out var listNode))
                    {
                        wikiNodes.Add(listNode!);
                    }
                    else if (TableNode.TryParse(elem, out var tableNode))
                    {
                        wikiNodes.Add(tableNode!);
                    }
                    else wikiNodes.AddRange(Extract(elem));
                }
            }
        }
        return wikiNodes;
    }
}