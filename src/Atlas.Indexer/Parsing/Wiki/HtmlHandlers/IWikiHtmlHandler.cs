using System;
using AngleSharp.Dom;

namespace Atlas.Indexer.Parsing.Wiki.HtmlHandlers;

public interface IWikiHtmlHandler
{
    public WikiHtmlParseResult? Handle(INode node);
}
