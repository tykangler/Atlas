using System;
using AngleSharp.Dom;
using Atlas.Core.Extensions;
using Atlas.Core.Model.Annotations;

namespace Atlas.Core.Parser.Wiki.HtmlHandlers;

public class LinkHandler : IWikiHtmlHandler
{
    private const string LinkTag = "a";
    private const string InterLinkTitleAttr = "title";
    private const string HrefAttr = "href";

    public WikiHtmlParseResult? Handle(INode node)
    {
        if (node is not IElement element || !IsValid(element))
        {
            return null;
        }
        var parsed = node.TextContent;

        return new WikiHtmlParseResult(
            Parsed: parsed,
            Annotations: new AnnotationCollection([
                new LinkAnnotation(
                    StartIndex: 0,
                    EndIndex: parsed.Length,
                    Text: parsed,
                    Link: element.GetAttribute(InterLinkTitleAttr)!)
            ])
        );
    }

    private bool IsValid(IElement element) =>
        element.IsTag(LinkTag)
        && element.HasAttribute(HrefAttr)
        && element.HasAttribute(InterLinkTitleAttr);
}
