using System;
using AngleSharp.Dom;
using Atlas.Core.Model.Annotations;

namespace Atlas.Core.Parser.Wiki.HtmlHandlers;

public class TextHandler : IWikiHtmlHandler
{
    public WikiHtmlParseResult? Handle(INode node)
    {
        if (node is not IText text)
        {
            return null;
        }
        return new WikiHtmlParseResult(Parsed: text.TextContent, Annotations: AnnotationCollection.Empty);
    }
}
