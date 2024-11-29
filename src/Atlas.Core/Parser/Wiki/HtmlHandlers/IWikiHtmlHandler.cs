using System;
using AngleSharp.Dom;

namespace Atlas.Core.Parser.Wiki.HtmlHandlers;

public interface IWikiHtmlHandler
{
    public WikiHtmlParseResult? Handle(INode node);
}
