namespace Atlas.Core.Wiki.Extract.AST;

public interface IWikiExtractor
{
    public IEnumerable<WikiNode> Extract(string doc);
}