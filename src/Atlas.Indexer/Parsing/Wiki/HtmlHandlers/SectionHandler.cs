using System;
using AngleSharp.Dom;
using AngleSharp.Text;
using Atlas.Clients.Wiki.Extensions;
using Atlas.Indexer.Models.Annotations;

namespace Atlas.Indexer.Parsing.Wiki.HtmlHandlers;

public class SectionHandler : IWikiHtmlHandler
{
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
                new SectionAnnotation(
                        StartIndex: 0,
                        EndIndex: parsed.Length,
                        Text: parsed,
                        SectionLevel: GetSectionLevel(element))
            ])
        );
    }

    private bool IsValid(IElement element) => element.HasClass("mw-heading");

    private int GetSectionLevel(IElement element)
    {
        var sectionLevelString = element.ClassList
            .Where(cl => cl.StartsWith("mw-heading") && cl[^1].IsDigit())
            .FirstOrDefault();
        if (sectionLevelString != null && int.TryParse(sectionLevelString[^1].ToString(), out int sectionLevel))
        {
            return sectionLevel;
        }
        else
        {
            return 1;
        }
    }
}
