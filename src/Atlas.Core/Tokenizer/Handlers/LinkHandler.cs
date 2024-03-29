using AngleSharp.Dom;
using Atlas.Core.Extensions;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class LinkHandler : IHandler
{
    private const string HrefAttribute = "href";

    public bool CanHandle(INode node)
    {
        if (node is IElement element)
        {
            var isAnchor = element.TagName == "A";
            var hrefAttr = element.GetAttribute(HrefAttribute);
            var hasWikiHref = hrefAttr != null;
            return isAnchor && hasWikiHref;
        }
        else
        {
            return false;
        }
    }

    public WikiToken? Handle(INode node)
    {
        if (!(node is IElement element && CanHandle(node)))
        {
            return null;
        }

        string link = element.GetAttribute(HrefAttribute)!;
        string value = element.Text().NormalizeWhiteSpace();
        return new LinkToken(link, value, link.StartsWith("/wiki"));
    }

}
