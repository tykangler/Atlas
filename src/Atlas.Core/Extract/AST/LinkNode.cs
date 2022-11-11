using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Wiki.Extract.AST;

public class LinkNode : WikiNode
{
    private const string href = "href";

    public string Url { get; }
    public string Value { get; }
    public bool IsInterlink { get; }

    private static bool Validate(IElement elem)
    {
        var isAnchor = elem.TagName == "A";
        var hrefAttr = elem.GetAttribute(href);
        var hasWikiHref = hrefAttr != null;
        return isAnchor && hasWikiHref;
    }

    public static LinkNode? TryParse(IElement elem)
    {
        // moved Validate here instead of outside, since here we can guarantee
        // that the node is valid.
        if (Validate(elem))
        {
            string link = elem.GetAttribute(href)!; // validated above
            string value = elem.Text().NormalizeWhiteSpace();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return new LinkNode(link, value, link.StartsWith("/wiki"));
            }
        }
        return null;
    }

    public LinkNode(string url, string value, bool isInterlink)
    {
        Url = url;
        Value = value;
        IsInterlink = isInterlink;
    }

    public override void Accept(ASTVisitor visitor) => visitor.VisitLink(this);
}
