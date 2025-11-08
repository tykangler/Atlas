using System;

namespace Atlas.Indexer.Parsing.Wiki.HtmlHandlers;

public static class WikiHandlerFactory
{
    public static IEnumerable<IWikiHtmlHandler> Default => [new LinkHandler(), new SectionHandler(), new TextHandler()];
}
