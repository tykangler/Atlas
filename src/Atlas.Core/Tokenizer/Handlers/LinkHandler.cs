using AngleSharp.Dom;
using Atlas.Core.Extensions;
using Atlas.Core.Tokenizer.Token;

namespace Atlas.Core.Tokenizer.Handlers;

public class LinkHandler : IHandler
{
    private const string HrefAttribute = "href";
    private const string LinkTag = "A";

    public bool CanHandle(INode node)
    {
        if (node is IElement element)
        {
            var isAnchor = element.IsTag(LinkTag);
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
        string? value = element.Text()?.NormalizeWhiteSpace();
        // if the link is to a file or the link does not contain any text, we don't want to tokenize it.
        if (link.Contains("File:") || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }
        return new LinkToken(link, value, link.StartsWith("/wiki"));
    }

}
