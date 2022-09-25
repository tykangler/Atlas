namespace Atlas.Core.Wiki.Extract.AST;

using HtmlAgilityPack;

/// <summary>
/// Parses through an html page from wikipedia and builds an ordered collection of WikiNode's
/// with respect to their order in the wikipedia document.
/// WikiTokenType 
/// </summary>
public class HtmlWikiExtractor : IWikiExtractor
{
    string[] disallowClasses = {
        "infobox", "reflist", "reference", "reference-text"
    };
    string[] disallowTags = { "style", "sup" };

    public IEnumerable<WikiNode> Extract(string htmlDoc)
    {
        // parse html
        HtmlDocument doc = new();
        doc.LoadHtml(htmlDoc);
        var documentNode = doc.DocumentNode;

        // build node list
        return ExtractNode(documentNode.FirstChild); // root node div.mw-parser-output
    }

    public IEnumerable<WikiNode> ExtractNode(HtmlNode node)
    {
        var classes = node.GetClasses();
        if (disallowTags.Contains(node.Name) || disallowClasses.Any(classes.Contains))
            return Enumerable.Empty<WikiNode>();
        else if (SectionNode.DoesMatch(node))
            return new List<WikiNode> { SectionNode.Parse(node) };
        else if (LinkNode.DoesMatch(node))
            return new List<WikiNode> { LinkNode.Parse(node) };
        else if (ListNode.DoesMatch(node))
            return new List<WikiNode> { ListNode.Parse(node) };
        else if (TableNode.DoesMatch(node))
            return new List<WikiNode> { TableNode.Parse(node) };
        else if (TextNode.DoesMatch(node))
        {
            var parsedNode = TextNode.Parse(node);
            if (parsedNode != null)
            {
                return new List<WikiNode> { parsedNode };
            }
            return Enumerable.Empty<WikiNode>();
        }
        else return node.ChildNodes.SelectMany(ExtractNode);
    }
}