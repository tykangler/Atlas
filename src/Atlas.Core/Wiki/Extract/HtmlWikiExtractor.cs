namespace Atlas.Core.Wiki.Extract;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Atlas.Core.Wiki.Extract.AST;

/// <summary>
/// Parses through an html page from wikipedia and builds an ordered collection of WikiNode's
/// with respect to their order in the wikipedia document.
/// WikiTokenType 
/// </summary>
public class HtmlWikiExtractor : IWikiExtractor
{
    private readonly string[] disallowClasses = {
        "infobox", "reflist", "reference", "hatnote", "thumb"
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


    // TODO: Revise TryParse to set out var to non-null value
    // TODO: Find better way to set text values that don't involve creating new objects
    private List<WikiNode> Extract(IElement htmlElement)
    {
        List<WikiNode> wikiNodes = new();
        foreach (var childHtmlNode in htmlElement.ChildNodes)
        {
            if (TextNode.TryParse(childHtmlNode, out var textNode))
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
            else if (childHtmlNode is IElement elem)
            {
                if (!disallowTags.Contains(elem.TagName) &&
                    !StringsPartiallyContain(refs: disallowClasses, target: elem.ClassList))
                {
                    if (SectionNode.TryParse(elem, out var sectionNode))
                    {
                        wikiNodes.Add(sectionNode!);
                    }
                    else if (LinkNode.TryParse(elem, out var interlinkNode))
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
                    // TODO: Don't like that there are side effects
                    else AddRangeAndMergeTextNodes(wikiNodes, Extract(elem));
                }
            }
        }
        return wikiNodes;
    }

    /// <summary>
    /// Returns true if any string in <c>target</c> contains any string in <c>ref</c>
    /// </summary>
    /// <param name="refs">A list of strings that may be inner strings in target</param>
    /// <param name="target">A list of strings that will be checked against the strings in ref</param>
    /// <returns>true/false</returns>
    private bool StringsPartiallyContain(IEnumerable<string> refs, IEnumerable<string> target)
    {
        foreach (var refString in refs)
        {
            foreach (var targetStr in target)
            {
                if (targetStr.Contains(refString))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private TextNode MergeTextNodes(TextNode node1, TextNode node2) => new TextNode($"{node1.Value} {node2.Value}");

    private void AddRangeAndMergeTextNodes(List<WikiNode> wikiNodes, List<WikiNode> toAdd)
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