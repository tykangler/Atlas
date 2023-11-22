using AngleSharp.Dom;
using Atlas.Core.Extensions;

namespace Atlas.Core.Tokenizer.Token;

public class LinkToken : WikiToken
{
    private const string href = "href";

    public string Url { get; }
    public string Value { get; }
    public bool IsInterlink { get; }

    private static bool Validate(INode node)
    {
        if (node is IElement element)
        {
            var isAnchor = element.TagName == "A";
            var hrefAttr = element.GetAttribute(href);
            var hasWikiHref = hrefAttr != null;
            return isAnchor && hasWikiHref;
        }
        else
        {
            return false;
        }
    }

    public static LinkToken? TryParse(INode node)
    {
        // moved Validate here instead of outside, since here we can guarantee
        // that the node is valid.
        if (node is IElement element && Validate(node))
        {
            string link = element.GetAttribute(href)!; // validated above
            string value = element.Text().NormalizeWhiteSpace();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return new LinkToken(link, value, link.StartsWith("/wiki"));
            }
        }
        return null;
    }

    public LinkToken(string url, string value, bool isInterlink)
    {
        Url = url;
        Value = value;
        IsInterlink = isInterlink;
    }

    public override void Accept(TokenVisitor visitor) => visitor.VisitLink(this);
}
