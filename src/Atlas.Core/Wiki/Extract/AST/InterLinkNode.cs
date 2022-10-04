using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Extract.AST;

public class InterLinkNode : WikiNode
{
    private const string href = "href";

    public string Url { get; }
    public string Value { get; }

    private static bool Validate(IElement elem)
    {
        var isAnchor = elem.TagName == "A";

        var hrefAttr = elem.GetAttribute(href);
        var hasWikiHref = hrefAttr != null && hrefAttr.Contains("/wiki/");

        var childNodes = elem.ChildNodes;
        var hasSingleText = childNodes.Count() == 1 &&
            childNodes.First().NodeType == NodeType.Text &&
            !string.IsNullOrWhiteSpace(childNodes.First().Text());
        return isAnchor && hasWikiHref && hasSingleText;
    }

    public static bool TryParse(IElement elem, out InterLinkNode? wikiNode)
    {
        // moved Validate here instead of outside, since here we can guarantee
        // that the node is valid.
        if (Validate(elem))
        {
            string link = elem.GetAttribute(href)!; // validated above
            wikiNode = new InterLinkNode(link, elem.ChildNodes.First().TextContent.NormalizeWhiteSpace());
            return true;
        }
        wikiNode = null;
        return false;
    }

    public InterLinkNode(string url, string value)
    {
        Url = url;
        Value = value;
    }

    public override void Accept(ASTVisitor visitor)
    {
        visitor.VisitInterLink(this);
    }
}
