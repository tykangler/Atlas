using System;

namespace Atlas.Core.Parser.Wiki.HtmlHandlers;

public static class WikiHandlerFactory
{
    public static IEnumerable<IWikiHtmlHandler> Default => [new LinkHandler(), new SectionHandler(), new TextHandler()];
}
