namespace Atlas.Core.Wiki.Annotator;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Atlas.Core.Extensions;
using Atlas.Core.Wiki.Annotator.Token;

/// <summary>
/// Parses through an html page from wikipedia and builds an ordered collection of WikiToken's
/// with respect to their order in the wikipedia document.
/// WikiTokenType 
/// </summary>
public class HtmlWikiAnotator : IHtmlAnnotator
{
    private readonly string[] disallowClasses = {
        "infobox", "reflist", "reference", "hatnote", "thumb", "noprint"
    };
    private readonly string[] disallowTags = { "style", "sup", "img", "table", "cite" };

    private HtmlParser htmlParser;

    public HtmlWikiAnotator()
    {
        htmlParser = new HtmlParser(new HtmlParserOptions
        {
            IsEmbedded = true
        });
    }

    public async Task<IEnumerable<WikiToken>> Extract(string htmlDoc)
    {
        var document = await this.htmlParser.ParseDocumentAsync(htmlDoc);
        if (document.Body?.FirstElementChild != null)
        {
            // build node list
            return ExtractFromHtml(document.Body.FirstElementChild); // div.mw-parser-output
        }
        return Enumerable.Empty<WikiToken>();
    }


    // design: Find better way to set text values that don't involve creating new objects
    // FIXME: Filter out unallowed tags and classes in node parsing methods (list)
    private List<WikiToken> ExtractFromHtml(IElement htmlElement)
    {
        List<WikiToken> wikiNodes = new();
        foreach (var childHtmlNode in htmlElement.ChildNodes)
        {
            if (TextNode.TryParse(childHtmlNode) is TextNode textNode)
            {
                if (wikiNodes.Count > 0 && wikiNodes[^1] is TextNode previousTextNode)
                {
                    // don't like that I create another element just to take it's value.
                    wikiNodes[^1] = MergeTextNodes(previousTextNode, textNode!);
                }
                else
                {
                    wikiNodes.Add(textNode!); // want to merge text nodes
                }
            }
            else if (childHtmlNode is IElement elem && ElementIsValid(elem))
            {
                if (SectionNode.TryParse(elem) is SectionNode sectionNode)
                {
                    wikiNodes.Add(sectionNode);
                }
                else if (LinkNode.TryParse(elem) is LinkNode linkNode)
                {
                    wikiNodes.Add(linkNode);
                }
                else if (ListNode.TryParse(elem, ExtractFromHtml) is ListNode listNode)
                {
                    wikiNodes.Add(listNode);
                }
                else if (TableNode.TryParse(elem) is TableNode tableNode)
                {
                    wikiNodes.Add(tableNode);
                }
                // design: Don't like that there are side effects to wikiNodes
                else
                {
                    var wikiNodesToAdd = ExtractFromHtml(elem);
                    AddRangeAndMergeTextNodes(wikiNodes, wikiNodesToAdd);
                }
            }
        }
        return wikiNodes;
    }

    private bool ElementIsValid(IElement elem) =>
        !disallowTags.Contains(elem.TagName, StringComparer.OrdinalIgnoreCase) &&
        !disallowClasses.Any(cl =>
            elem.ClassList.PartiallyContains(cl, StringComparison.OrdinalIgnoreCase));

    private TextNode MergeTextNodes(TextNode node1, TextNode node2) => new TextNode($"{node1.Value} {node2.Value}");

    private void AddRangeAndMergeTextNodes(List<WikiToken> wikiNodes, List<WikiToken> toAdd)
    {
        if (toAdd.Count > 0 && toAdd[0] is TextNode textNode &&
            wikiNodes.Count > 0 && wikiNodes[^1] is TextNode lastTextNode)
        {
            wikiNodes[^1] = MergeTextNodes(lastTextNode, textNode);
            wikiNodes.AddRange(toAdd.Skip(1));
        }
        else
        {
            wikiNodes.AddRange(toAdd);
        }
    }
}